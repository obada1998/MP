using Platform.Application.Categories;
using Platform.Application.Pages;
using Platform.Application.Products;
using Platform.Application.Stores;

namespace Platform.Application.Storefront;

public sealed class StorefrontPageDto
{
    public StoreDto Store { get; set; } = new();
    public PageDto Page { get; set; } = new();
    public PageLayoutDocument Layout { get; set; } = new();
    public string LayoutJson { get; set; } = "{}";
}

public sealed class StorefrontCatalogDto
{
    public StoreDto Store { get; set; } = new();
    public IReadOnlyCollection<ProductDto> Products { get; set; } = [];
    public IReadOnlyCollection<CategoryDto> Categories { get; set; } = [];
}

public sealed class StorefrontNavigationDto
{
    public StoreDto Store { get; set; } = new();
    public IReadOnlyCollection<StorefrontNavigationPageDto> Pages { get; set; } = [];
}

public sealed class StorefrontNavigationPageDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
}
