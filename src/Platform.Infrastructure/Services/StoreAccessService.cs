using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Auth;
using Platform.Application.Common;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Identity;

namespace Platform.Infrastructure.Services;

public sealed class StoreAccessService(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager) : IStoreAccessService
{
    public async Task EnsureStoreAccessAsync(
        string userId,
        Guid storeId,
        IReadOnlyCollection<StoreMembershipRole> allowedRoles,
        CancellationToken cancellationToken = default)
    {
        if (!await HasStoreAccessAsync(userId, storeId, allowedRoles, cancellationToken))
        {
            throw new ForbiddenAccessException("The current user cannot access this store.");
        }
    }

    public async Task<bool> HasStoreAccessAsync(
        string userId,
        Guid storeId,
        IReadOnlyCollection<StoreMembershipRole> allowedRoles,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var membershipRoles = await dbContext.StoreUsers
            .Where(x => x.StoreId == storeId && x.UserId == userId && x.IsActive)
            .Select(x => x.Role)
            .ToListAsync(cancellationToken);
        if (membershipRoles.Any(allowedRoles.Contains))
        {
            return true;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        return await userManager.IsInRoleAsync(user, PlatformRoles.PlatformAdmin);
    }
}
