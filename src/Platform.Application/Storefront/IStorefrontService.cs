using Platform.Application.Categories;
using Platform.Application.Products;

namespace Platform.Application.Storefront;

public interface IStorefrontService
{
    Task<StorefrontPageDto> GetHomePageAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default);
    Task<StorefrontPageDto> GetPageAsync(string storeSlugOrDomain, string pageSlug, CancellationToken cancellationToken = default);
    Task<StorefrontNavigationDto> GetNavigationAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default);
    Task<StorefrontCatalogDto> GetCatalogAsync(string storeSlugOrDomain, Guid? categoryId = null, CancellationToken cancellationToken = default);
    Task<ProductDto> GetProductAsync(string storeSlugOrDomain, string productSlug, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync(string storeSlugOrDomain, CancellationToken cancellationToken = default);
}
