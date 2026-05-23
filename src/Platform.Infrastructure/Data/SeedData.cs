using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Auth;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Identity;

namespace Platform.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var dbContext = provider.GetRequiredService<ApplicationDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync();

        foreach (var role in PlatformRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await EnsureUserAsync(userManager, "admin@platform.local", "Platform Admin", PlatformRoles.PlatformAdmin, resetPassword: true);
        var owner = await EnsureUserAsync(userManager, "owner@demo.local", "Demo Store Owner", PlatformRoles.StoreOwner, resetPassword: true);
        await EnsureUserAsync(userManager, "customer@demo.local", "Demo Customer", PlatformRoles.Customer, resetPassword: true);

        if (await dbContext.Stores.AnyAsync())
        {
            return;
        }

        var store = new Store
        {
            Name = "Demo Store",
            Slug = "demo-store",
            ThemeName = "Modern",
            LogoUrl = "/images/demo-logo.png",
            IsActive = true
        };
        dbContext.Stores.Add(store);
        dbContext.StoreUsers.Add(new StoreUser
        {
            StoreId = store.Id,
            UserId = owner.Id,
            Role = StoreMembershipRole.StoreOwner
        });
        dbContext.StoreSettings.Add(new StoreSettings
        {
            StoreId = store.Id,
            Currency = "USD",
            Culture = "en-US",
            ContactEmail = "support@demo.local"
        });
        dbContext.ThemeSettings.Add(new ThemeSettings
        {
            StoreId = store.Id,
            ThemeName = "Modern",
            PrimaryColor = "#2563eb",
            AccentColor = "#f97316"
        });

        var apparel = new Category { StoreId = store.Id, Name = "Apparel", Slug = "apparel", SortOrder = 1 };
        var accessories = new Category { StoreId = store.Id, Name = "Accessories", Slug = "accessories", SortOrder = 2 };
        dbContext.Categories.AddRange(apparel, accessories);

        var colorField = new ProductFieldDefinition
        {
            StoreId = store.Id,
            Key = "color",
            Label = "Color",
            FieldType = ProductFieldType.Color,
            IsVisibleOnListing = true,
            DisplayOrder = 10
        };
        var sizeField = new ProductFieldDefinition
        {
            StoreId = store.Id,
            Key = "size",
            Label = "Size",
            FieldType = ProductFieldType.Select,
            IsVisibleOnListing = true,
            DisplayOrder = 20,
            OptionsJson = """{"options":["S","M","L","XL"]}"""
        };
        dbContext.ProductFieldDefinitions.AddRange(colorField, sizeField);

        var product = new Product
        {
            StoreId = store.Id,
            Name = "Essential Hoodie",
            Slug = "essential-hoodie",
            Sku = "HD-001",
            Description = "Soft midweight hoodie used by seed data and storefront previews.",
            BasePrice = 59,
            Status = ProductStatus.Active,
            PrimaryImageUrl = "https://placehold.co/900x700?text=Essential+Hoodie"
        };
        product.ProductCategories.Add(new ProductCategory { StoreId = store.Id, ProductId = product.Id, CategoryId = apparel.Id });
        product.CustomFieldValues.Add(new ProductCustomFieldValue { StoreId = store.Id, ProductId = product.Id, ProductFieldDefinitionId = colorField.Id, ValueJson = "\"#374151\"" });
        product.CustomFieldValues.Add(new ProductCustomFieldValue { StoreId = store.Id, ProductId = product.Id, ProductFieldDefinitionId = sizeField.Id, ValueJson = "\"M\"" });
        dbContext.Products.Add(product);

        var home = new Page
        {
            StoreId = store.Id,
            Title = "Home",
            Slug = "home",
            IsHomePage = true,
            IsPublished = true,
            PublishedAt = DateTimeOffset.UtcNow
        };
        dbContext.Pages.Add(home);
        dbContext.PageLayouts.Add(new PageLayout
        {
            StoreId = store.Id,
            PageId = home.Id,
            Version = 1,
            Status = PageLayoutStatus.Published,
            PublishedAt = DateTimeOffset.UtcNow,
            LayoutJson = """
            {"pageId":"home","sections":[{"type":"hero","order":1,"props":{"title":"Welcome to Demo Store","subtitle":"A metadata-driven storefront rendered from JSON.","imageUrl":"https://placehold.co/1600x700?text=Demo+Store","buttonText":"Shop now","buttonUrl":"/store/demo-store/products"},"styles":{}},{"type":"productGrid","order":2,"props":{"columns":4,"limit":8},"styles":{}}]}
            """
        });

        _ = admin;
        await dbContext.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string displayName,
        string role,
        bool resetPassword = false)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName
            };

            var result = await userManager.CreateAsync(user, "Password123!");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        if (resetPassword && !await userManager.CheckPasswordAsync(user, "Password123!"))
        {
            if (await userManager.HasPasswordAsync(user))
            {
                var removeResult = await userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", removeResult.Errors.Select(x => x.Description)));
                }
            }

            var addResult = await userManager.AddPasswordAsync(user, "Password123!");
            if (!addResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", addResult.Errors.Select(x => x.Description)));
            }
        }

        return user;
    }
}
