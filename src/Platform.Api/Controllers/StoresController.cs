using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Stores;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize]
public sealed class StoresController(IStoreService storeService) : ApiControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyCollection<StoreDto>>> Mine(CancellationToken cancellationToken)
    {
        return Ok(await storeService.GetStoresForUserAsync(CurrentUserId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create(CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var store = await storeService.CreateAsync(CurrentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { storeId = store.Id }, store);
    }

    [HttpGet("{storeId:guid}")]
    public async Task<ActionResult<StoreDto>> Get(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await storeService.GetAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPut("{storeId:guid}")]
    public async Task<ActionResult<StoreDto>> Update(Guid storeId, UpdateStoreRequest request, CancellationToken cancellationToken)
    {
        return Ok(await storeService.UpdateAsync(CurrentUserId, storeId, request, cancellationToken));
    }

    [HttpGet("{storeId:guid}/settings")]
    public async Task<ActionResult<StoreSettingsDto>> GetSettings(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await storeService.GetSettingsAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPut("{storeId:guid}/settings")]
    public async Task<ActionResult<StoreSettingsDto>> UpdateSettings(Guid storeId, StoreSettingsDto request, CancellationToken cancellationToken)
    {
        return Ok(await storeService.UpdateSettingsAsync(CurrentUserId, storeId, request, cancellationToken));
    }

    [HttpGet("{storeId:guid}/theme")]
    public async Task<ActionResult<ThemeSettingsDto>> GetTheme(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await storeService.GetThemeAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPut("{storeId:guid}/theme")]
    public async Task<ActionResult<ThemeSettingsDto>> UpdateTheme(Guid storeId, ThemeSettingsDto request, CancellationToken cancellationToken)
    {
        return Ok(await storeService.UpdateThemeAsync(CurrentUserId, storeId, request, cancellationToken));
    }
}
