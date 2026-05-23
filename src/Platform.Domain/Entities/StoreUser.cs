using Platform.Domain.Common;
using Platform.Domain.Enums;

namespace Platform.Domain.Entities;

public sealed class StoreUser : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public StoreMembershipRole Role { get; set; } = StoreMembershipRole.StoreStaff;
    public bool IsActive { get; set; } = true;
}
