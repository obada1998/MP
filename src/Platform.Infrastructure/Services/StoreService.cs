using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Stores;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class StoreService(
    ApplicationDbContext dbContext,
    IStoreAccessService storeAccessService) : IStoreService
{
    private static readonly StoreMembershipRole[] OwnerOnly = [StoreMembershipRole.StoreOwner];

    public async Task<IReadOnlyCollection<StoreDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Stores
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StoreDto>> GetStoresForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var memberships = await dbContext.StoreUsers
            .AsNoTracking()
            .Include(x => x.Store)
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderBy(x => x.Store.Name)
            .ToListAsync(cancellationToken);

        return memberships.Select(x =>
        {
            var dto = x.Store.ToDto();
            dto.CurrentUserRole = x.Role;
            return dto;
        }).ToArray();
    }

    public async Task<StoreDto> CreateAsync(string ownerUserId, CreateStoreRequest request, CancellationToken cancellationToken = default)
    {
        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        if (await dbContext.Stores.AnyAsync(x => x.Slug == slug, cancellationToken))
        {
            throw new InvalidOperationException("A store already exists with this slug.");
        }

        var store = new Store
        {
            Name = request.Name.Trim(),
            Slug = slug,
            LogoUrl = request.LogoUrl,
            Domain = string.IsNullOrWhiteSpace(request.Domain) ? null : request.Domain.Trim().ToLowerInvariant(),
            ThemeName = request.ThemeName.Trim()
        };

        dbContext.Stores.Add(store);
        dbContext.StoreUsers.Add(new StoreUser
        {
            StoreId = store.Id,
            UserId = ownerUserId,
            Role = StoreMembershipRole.StoreOwner
        });
        dbContext.StoreSettings.Add(new StoreSettings { StoreId = store.Id });
        dbContext.ThemeSettings.Add(new ThemeSettings { StoreId = store.Id, ThemeName = store.ThemeName });

        await dbContext.SaveChangesAsync(cancellationToken);
        var dto = store.ToDto();
        dto.CurrentUserRole = StoreMembershipRole.StoreOwner;
        return dto;
    }

    public async Task<StoreDto> GetAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff], cancellationToken);
        var store = await dbContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.Id == storeId, cancellationToken)
            ?? throw new KeyNotFoundException("Store not found.");
        return store.ToDto();
    }

    public async Task<StoreDto> UpdateAsync(string userId, Guid storeId, UpdateStoreRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, OwnerOnly, cancellationToken);
        var store = await dbContext.Stores.SingleOrDefaultAsync(x => x.Id == storeId, cancellationToken)
            ?? throw new KeyNotFoundException("Store not found.");

        store.Name = request.Name.Trim();
        store.LogoUrl = request.LogoUrl;
        store.Domain = string.IsNullOrWhiteSpace(request.Domain) ? null : request.Domain.Trim().ToLowerInvariant();
        store.ThemeName = request.ThemeName.Trim();
        store.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);
        return store.ToDto();
    }

    public async Task<StoreSettingsDto> GetSettingsAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff], cancellationToken);
        return (await EnsureSettingsAsync(storeId, cancellationToken)).ToDto();
    }

    public async Task<StoreSettingsDto> UpdateSettingsAsync(string userId, Guid storeId, StoreSettingsDto request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, OwnerOnly, cancellationToken);
        JsonGuards.EnsureValidJson(request.SettingsJson, nameof(request.SettingsJson));

        var settings = await EnsureSettingsAsync(storeId, cancellationToken);
        settings.Currency = request.Currency.Trim();
        settings.Culture = request.Culture.Trim();
        settings.ContactEmail = request.ContactEmail;
        settings.SettingsJson = string.IsNullOrWhiteSpace(request.SettingsJson) ? "{}" : request.SettingsJson;
        await dbContext.SaveChangesAsync(cancellationToken);
        return settings.ToDto();
    }

    public async Task<ThemeSettingsDto> GetThemeAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff], cancellationToken);
        return (await EnsureThemeAsync(storeId, cancellationToken)).ToDto();
    }

    public async Task<ThemeSettingsDto> UpdateThemeAsync(string userId, Guid storeId, ThemeSettingsDto request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, OwnerOnly, cancellationToken);
        JsonGuards.EnsureValidJson(request.SettingsJson, nameof(request.SettingsJson));

        var theme = await EnsureThemeAsync(storeId, cancellationToken);
        theme.ThemeName = request.ThemeName.Trim();
        theme.PrimaryColor = request.PrimaryColor.Trim();
        theme.AccentColor = request.AccentColor.Trim();
        theme.FontFamily = request.FontFamily.Trim();
        theme.CustomCss = request.CustomCss;
        theme.SettingsJson = string.IsNullOrWhiteSpace(request.SettingsJson) ? "{}" : request.SettingsJson;
        await dbContext.SaveChangesAsync(cancellationToken);
        return theme.ToDto();
    }

    private async Task<StoreSettings> EnsureSettingsAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var settings = await dbContext.StoreSettings.SingleOrDefaultAsync(x => x.StoreId == storeId, cancellationToken);
        if (settings is not null)
        {
            return settings;
        }

        settings = new StoreSettings { StoreId = storeId };
        dbContext.StoreSettings.Add(settings);
        await dbContext.SaveChangesAsync(cancellationToken);
        return settings;
    }

    private async Task<ThemeSettings> EnsureThemeAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var theme = await dbContext.ThemeSettings.SingleOrDefaultAsync(x => x.StoreId == storeId, cancellationToken);
        if (theme is not null)
        {
            return theme;
        }

        theme = new ThemeSettings { StoreId = storeId };
        dbContext.ThemeSettings.Add(theme);
        await dbContext.SaveChangesAsync(cancellationToken);
        return theme;
    }
}
