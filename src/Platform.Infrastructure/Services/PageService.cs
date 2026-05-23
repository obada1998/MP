using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Pages;
using Platform.Application.Rendering;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class PageService(
    ApplicationDbContext dbContext,
    IStoreAccessService storeAccessService,
    PageLayoutValidator layoutValidator) : IPageService
{
    private static readonly StoreMembershipRole[] PageEditors = [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff];

    public async Task<IReadOnlyCollection<PageDto>> GetPagesAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var pages = await dbContext.Pages
            .AsNoTracking()
            .Where(x => x.StoreId == storeId)
            .OrderByDescending(x => x.IsHomePage)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
        return pages.Select(x => x.ToDto()).ToArray();
    }

    public async Task<PageDto> CreatePageAsync(string userId, Guid storeId, UpsertPageRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug);
        await EnsureUniqueSlugAsync(storeId, slug, null, cancellationToken);

        if (request.IsHomePage)
        {
            await ClearExistingHomepageAsync(storeId, cancellationToken);
        }

        var page = new Page
        {
            StoreId = storeId,
            Title = request.Title.Trim(),
            Slug = slug,
            IsHomePage = request.IsHomePage,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTimeOffset.UtcNow : null
        };
        dbContext.Pages.Add(page);
        await dbContext.SaveChangesAsync(cancellationToken);
        return page.ToDto();
    }

    public async Task<PageDto> UpdatePageAsync(string userId, Guid storeId, Guid pageId, UpsertPageRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var page = await dbContext.Pages.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == pageId, cancellationToken)
            ?? throw new KeyNotFoundException("Page not found.");

        var slug = SlugGenerator.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug);
        await EnsureUniqueSlugAsync(storeId, slug, pageId, cancellationToken);
        if (request.IsHomePage)
        {
            await ClearExistingHomepageAsync(storeId, cancellationToken, pageId);
        }

        page.Title = request.Title.Trim();
        page.Slug = slug;
        page.IsHomePage = request.IsHomePage;
        page.IsPublished = request.IsPublished;
        page.PublishedAt = request.IsPublished ? page.PublishedAt ?? DateTimeOffset.UtcNow : null;
        await dbContext.SaveChangesAsync(cancellationToken);
        return page.ToDto();
    }

    public async Task DeletePageAsync(string userId, Guid storeId, Guid pageId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var page = await dbContext.Pages.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == pageId, cancellationToken)
            ?? throw new KeyNotFoundException("Page not found.");
        dbContext.Pages.Remove(page);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PageLayoutDto?> GetLatestLayoutAsync(string userId, Guid storeId, Guid pageId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var layout = await dbContext.PageLayouts
            .AsNoTracking()
            .Where(x => x.StoreId == storeId && x.PageId == pageId)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
        return layout?.ToDto();
    }

    public async Task<PageLayoutDto> SaveLayoutAsync(string userId, Guid storeId, Guid pageId, SavePageLayoutRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, PageEditors, cancellationToken);
        var page = await dbContext.Pages.SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == pageId, cancellationToken)
            ?? throw new KeyNotFoundException("Page not found.");

        var validation = layoutValidator.Validate(request.LayoutJson);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(string.Join("; ", validation.Errors));
        }

        var nextVersion = await dbContext.PageLayouts
            .Where(x => x.StoreId == storeId && x.PageId == pageId)
            .Select(x => (int?)x.Version)
            .MaxAsync(cancellationToken) ?? 0;

        if (request.Publish)
        {
            var published = await dbContext.PageLayouts
                .Where(x => x.StoreId == storeId && x.PageId == pageId && x.Status == PageLayoutStatus.Published)
                .ToListAsync(cancellationToken);
            foreach (var existing in published)
            {
                existing.Status = PageLayoutStatus.Archived;
            }
        }

        var layout = new PageLayout
        {
            StoreId = storeId,
            PageId = pageId,
            Version = nextVersion + 1,
            Status = request.Publish ? PageLayoutStatus.Published : PageLayoutStatus.Draft,
            PublishedAt = request.Publish ? DateTimeOffset.UtcNow : null,
            LayoutJson = validation.NormalizedJson
        };
        dbContext.PageLayouts.Add(layout);

        if (request.Publish)
        {
            page.IsPublished = true;
            page.PublishedAt = layout.PublishedAt;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return layout.ToDto();
    }

    public async Task<PageLayoutDto?> GetPublishedLayoutAsync(Guid storeId, string slug, CancellationToken cancellationToken = default)
    {
        var page = await dbContext.Pages
            .AsNoTracking()
            .Where(x => x.StoreId == storeId && x.Slug == slug && x.IsPublished)
            .SingleOrDefaultAsync(cancellationToken);
        if (page is null)
        {
            return null;
        }

        var layout = await dbContext.PageLayouts
            .AsNoTracking()
            .Where(x => x.StoreId == storeId && x.PageId == page.Id && x.Status == PageLayoutStatus.Published)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
        return layout?.ToDto();
    }

    private async Task ClearExistingHomepageAsync(Guid storeId, CancellationToken cancellationToken, Guid? excludingPageId = null)
    {
        var pages = await dbContext.Pages
            .Where(x => x.StoreId == storeId && x.IsHomePage && (!excludingPageId.HasValue || x.Id != excludingPageId.Value))
            .ToListAsync(cancellationToken);
        foreach (var page in pages)
        {
            page.IsHomePage = false;
        }
    }

    private async Task EnsureUniqueSlugAsync(Guid storeId, string slug, Guid? excludingPageId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Pages.AnyAsync(
            x => x.StoreId == storeId && x.Slug == slug && (!excludingPageId.HasValue || x.Id != excludingPageId),
            cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("A page already exists with this slug.");
        }
    }
}
