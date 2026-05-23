using Platform.Domain.Enums;

namespace Platform.Application.Abstractions;

public interface IStoreAccessService
{
    Task EnsureStoreAccessAsync(
        string userId,
        Guid storeId,
        IReadOnlyCollection<StoreMembershipRole> allowedRoles,
        CancellationToken cancellationToken = default);

    Task<bool> HasStoreAccessAsync(
        string userId,
        Guid storeId,
        IReadOnlyCollection<StoreMembershipRole> allowedRoles,
        CancellationToken cancellationToken = default);
}
