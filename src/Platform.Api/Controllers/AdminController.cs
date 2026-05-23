using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Auth;
using Platform.Application.Stores;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = PlatformRoles.PlatformAdmin)]
public sealed class AdminController(IStoreService storeService) : ControllerBase
{
    [HttpGet("stores")]
    public async Task<ActionResult<IReadOnlyCollection<StoreDto>>> Stores(CancellationToken cancellationToken)
    {
        return Ok(await storeService.GetAllAsync(cancellationToken));
    }
}
