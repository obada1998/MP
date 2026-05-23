using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class ProductImage : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
}
