using Platform.Domain.Common;
using Platform.Domain.Enums;

namespace Platform.Domain.Entities;

public sealed class Product : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public string? PrimaryImageUrl { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductCustomFieldValue> CustomFieldValues { get; set; } = new List<ProductCustomFieldValue>();
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
