using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Abstractions;
using Platform.Application.Auth;
using Platform.Application.Products;
using Platform.Application.Rendering;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Services;

namespace Platform.Tests.Infrastructure;

internal static class TestHost
{
    public static ServiceProvider Create()
    {
        var services = new ServiceCollection();
        var databaseName = Guid.NewGuid().ToString();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<PageLayoutValidator>();
        services.AddScoped<IStoreAccessService, StoreAccessService>();
        services.AddScoped<IProductService, ProductService>();
        return services.BuildServiceProvider();
    }

    public static async Task<(string OwnerId, Guid StoreId, Guid OtherStoreId)> SeedTenantsAsync(ServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var role in PlatformRoles.All)
        {
            await roles.CreateAsync(new IdentityRole(role));
        }

        var owner = new ApplicationUser { UserName = "owner@test.local", Email = "owner@test.local", DisplayName = "Owner" };
        var otherOwner = new ApplicationUser { UserName = "other@test.local", Email = "other@test.local", DisplayName = "Other" };
        await users.CreateAsync(owner, "Password123!");
        await users.CreateAsync(otherOwner, "Password123!");

        var store = new Store { Name = "Store A", Slug = "store-a" };
        var otherStore = new Store { Name = "Store B", Slug = "store-b" };
        db.Stores.AddRange(store, otherStore);
        db.StoreUsers.AddRange(
            new StoreUser { StoreId = store.Id, UserId = owner.Id, Role = StoreMembershipRole.StoreOwner },
            new StoreUser { StoreId = otherStore.Id, UserId = otherOwner.Id, Role = StoreMembershipRole.StoreOwner });
        await db.SaveChangesAsync();

        return (owner.Id, store.Id, otherStore.Id);
    }
}
