using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class Category : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
