using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class StoreSettings : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Currency { get; set; } = "USD";
    public string Culture { get; set; } = "en-US";
    public string? ContactEmail { get; set; }
    public string SettingsJson { get; set; } = "{}";
}
