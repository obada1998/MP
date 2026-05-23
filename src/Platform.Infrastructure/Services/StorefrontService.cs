using Microsoft.EntityFrameworkCore;
using Platform.Application.Categories;
using Platform.Application.Products;
using Platform.Application.Storefront;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class StorefrontService(ApplicationDbContext dbContext) : IStorefrontService
{
    public async Task<StorefrontPageDto> GetHomePageAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var page = await dbContext.Pages
            .AsNoTracking()
            .Where(x => x.StoreId == store.Id && x.IsHomePage && x.IsPublished)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Homepage not found.");
        return await BuildPageAsync(store, page, cancellationToken);
    }

    public async Task<StorefrontPageDto> GetPageAsync(string storeSlugOrDomain, string pageSlug, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var page = await dbContext.Pages
            .AsNoTracking()
            .Where(x => x.StoreId == store.Id && x.Slug == pageSlug && x.IsPublished)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Page not found.");
        return await BuildPageAsync(store, page, cancellationToken);
    }

    public async Task<StorefrontNavigationDto> GetNavigationAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var pages = await dbContext.Pages
            .AsNoTracking()
            .Where(x => x.StoreId == store.Id && x.IsPublished)
            .OrderByDescending(x => x.IsHomePage)
            .ThenBy(x => x.Title)
            .Select(x => new StorefrontNavigationPageDto
            {
                Title = x.Title,
                Slug = x.Slug,
                IsHomePage = x.IsHomePage
            })
            .ToListAsync(cancellationToken);

        return new StorefrontNavigationDto
        {
            Store = store.ToDto(),
            Pages = pages
        };
    }

    public async Task<StorefrontCatalogDto> GetCatalogAsync(string storeSlugOrDomain, Guid? categoryId = null, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var productQuery = ProductQuery(store.Id).Where(x => x.Status == ProductStatus.Active);
        if (categoryId.HasValue)
        {
            productQuery = productQuery.Where(x => x.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));
        }

        var products = await productQuery.OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var categories = await CategoryQuery(store.Id).Where(x => x.IsActive).ToListAsync(cancellationToken);

        return new StorefrontCatalogDto
        {
            Store = store.ToDto(),
            Products = products.Select(x => x.ToDto()).ToArray(),
            Categories = categories.Select(x => x.ToDto()).ToArray()
        };
    }

    public async Task<ProductDto> GetProductAsync(string storeSlugOrDomain, string productSlug, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var product = await ProductQuery(store.Id)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Slug == productSlug && x.Status == ProductStatus.Active, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");
        return product.ToDto();
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default)
    {
        var store = await ResolveStoreAsync(storeSlugOrDomain, cancellationToken);
        var categories = await CategoryQuery(store.Id)
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return categories.Select(x => x.ToDto()).ToArray();
    }

    private async Task<StorefrontPageDto> BuildPageAsync(Store store, Page page, CancellationToken cancellationToken)
    {
        var layout = await dbContext.PageLayouts
            .AsNoTracking()
            .Where(x => x.StoreId == store.Id && x.PageId == page.Id && x.Status == PageLayoutStatus.Published)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Published page layout not found.");

        var dto = layout.ToDto();
        return new StorefrontPageDto
        {
            Store = store.ToDto(),
            Page = page.ToDto(),
            Layout = dto.Layout,
            LayoutJson = dto.LayoutJson
        };
    }

    private async Task<Store> ResolveStoreAsync(string storeSlugOrDomain, CancellationToken cancellationToken)
    {
        var key = storeSlugOrDomain.Trim().ToLowerInvariant();
        return await dbContext.Stores
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.IsActive && (x.Slug == key || x.Domain == key), cancellationToken)
            ?? throw new KeyNotFoundException("Store not found.");
    }

    private IQueryable<Product> ProductQuery(Guid storeId) => dbContext.Products
        .Where(x => x.StoreId == storeId)
        .Include(x => x.Images)
        .Include(x => x.ProductCategories)
        .Include(x => x.CustomFieldValues)
        .ThenInclude(x => x.FieldDefinition);

    private IQueryable<Category> CategoryQuery(Guid storeId) => dbContext.Categories
        .Where(x => x.StoreId == storeId)
        .Include(x => x.ProductCategories);
}
