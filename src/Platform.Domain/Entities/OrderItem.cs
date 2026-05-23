using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class OrderItem : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public string CustomSnapshotJson { get; set; } = "{}";
}
