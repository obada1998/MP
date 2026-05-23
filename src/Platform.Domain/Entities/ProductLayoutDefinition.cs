using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class ProductLayoutDefinition : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Name { get; set; } = "Default";
    public bool IsDefault { get; set; } = true;
    public string LayoutJson { get; set; } = "{}";
}
