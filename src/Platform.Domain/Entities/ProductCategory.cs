using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class ProductCategory : IStoreScoped
{
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
