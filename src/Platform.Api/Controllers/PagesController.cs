using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Pages;
using Platform.Application.Rendering;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/stores/{storeId:guid}/pages")]
[Authorize]
public sealed class PagesController(IPageService pageService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PageDto>>> Get(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await pageService.GetPagesAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<PageDto>> Create(Guid storeId, UpsertPageRequest request, CancellationToken cancellationToken)
    {
        return Ok(await pageService.CreatePageAsync(CurrentUserId, storeId, request, cancellationToken));
    }

    [HttpPut("{pageId:guid}")]
    public async Task<ActionResult<PageDto>> Update(Guid storeId, Guid pageId, UpsertPageRequest request, CancellationToken cancellationToken)
    {
        return Ok(await pageService.UpdatePageAsync(CurrentUserId, storeId, pageId, request, cancellationToken));
    }

    [HttpDelete("{pageId:guid}")]
    public async Task<IActionResult> Delete(Guid storeId, Guid pageId, CancellationToken cancellationToken)
    {
        await pageService.DeletePageAsync(CurrentUserId, storeId, pageId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{pageId:guid}/layout")]
    public async Task<ActionResult<PageLayoutDto?>> GetLayout(Guid storeId, Guid pageId, CancellationToken cancellationToken)
    {
        return Ok(await pageService.GetLatestLayoutAsync(CurrentUserId, storeId, pageId, cancellationToken));
    }

    [HttpPost("{pageId:guid}/layout")]
    public async Task<ActionResult<PageLayoutDto>> SaveLayout(Guid storeId, Guid pageId, SavePageLayoutRequest request, CancellationToken cancellationToken)
    {
        return Ok(await pageService.SaveLayoutAsync(CurrentUserId, storeId, pageId, request, cancellationToken));
    }
}

[ApiController]
[Route("api/page-components")]
[AllowAnonymous]
public sealed class PageComponentsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<PageComponentDefinition>> Get()
    {
        return Ok(PageComponentCatalog.All);
    }
}
