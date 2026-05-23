using Microsoft.AspNetCore.Identity;
using Platform.Domain.Entities;

namespace Platform.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<StoreUser> StoreUsers { get; set; } = new List<StoreUser>();
}
