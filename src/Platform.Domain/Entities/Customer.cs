using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class Customer : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
