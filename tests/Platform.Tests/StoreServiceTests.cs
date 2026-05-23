using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Abstractions;
using Platform.Application.Pages;
using Platform.Application.Stores;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Services;
using Platform.Tests.Infrastructure;

namespace Platform.Tests;

public sealed class StoreServiceTests
{
    [Fact]
    public async Task CreateAsync_creates_selected_default_pages_with_published_layouts()
    {
        await using var provider = TestHost.Create();

        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var storeService = new StoreService(db, new NoopStoreAccessService());

        var store = await storeService.CreateAsync("owner-1", new CreateStoreRequest
        {
            Name = "Demo Store",
            Slug = "demo-store",
            DefaultPages = [DefaultSystemPages.Home, DefaultSystemPages.Products, DefaultSystemPages.Contact]
        });

        var pages = db.Pages.Where(x => x.StoreId == store.Id).OrderBy(x => x.Slug).ToArray();
        var layouts = db.PageLayouts.Where(x => x.StoreId == store.Id).ToArray();

        Assert.Equal(3, pages.Length);
        Assert.Equal(3, layouts.Length);
        Assert.Contains(pages, x => x.Slug == "home" && x.IsHomePage && x.IsPublished);
        Assert.Contains(pages, x => x.Slug == "products");
        Assert.Contains(pages, x => x.Slug == "contact");
        Assert.All(layouts, x => Assert.Equal(PageLayoutStatus.Published, x.Status));
    }

    private sealed class NoopStoreAccessService : IStoreAccessService
    {
        public Task EnsureStoreAccessAsync(string userId, Guid storeId, IReadOnlyCollection<StoreMembershipRole> allowedRoles, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<bool> HasStoreAccessAsync(string userId, Guid storeId, IReadOnlyCollection<StoreMembershipRole> allowedRoles, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }
}
