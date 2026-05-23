using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Orders;
using Platform.Application.Storefront;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/storefront/{storeKey}")]
[AllowAnonymous]
public sealed class StorefrontController(
    IStorefrontService storefrontService,
    IOrderService orderService) : ControllerBase
{
    [HttpGet("home")]
    public async Task<ActionResult<StorefrontPageDto>> Home(string storeKey, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetHomePageAsync(storeKey, cancellationToken));
    }

    [HttpGet("pages/{slug}")]
    public async Task<ActionResult<StorefrontPageDto>> Page(string storeKey, string slug, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetPageAsync(storeKey, slug, cancellationToken));
    }

    [HttpGet("navigation")]
    public async Task<ActionResult<StorefrontNavigationDto>> Navigation(string storeKey, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetNavigationAsync(storeKey, cancellationToken));
    }

    [HttpGet("catalog")]
    public async Task<ActionResult<StorefrontCatalogDto>> Catalog(string storeKey, [FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetCatalogAsync(storeKey, categoryId, cancellationToken));
    }

    [HttpGet("products/{slug}")]
    public async Task<ActionResult> Product(string storeKey, string slug, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetProductAsync(storeKey, slug, cancellationToken));
    }

    [HttpGet("categories")]
    public async Task<ActionResult> Categories(string storeKey, CancellationToken cancellationToken)
    {
        return Ok(await storefrontService.GetCategoriesAsync(storeKey, cancellationToken));
    }

    [HttpPost("checkout")]
    public async Task<ActionResult> Checkout(string storeKey, CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var catalog = await storefrontService.GetCatalogAsync(storeKey, cancellationToken: cancellationToken);
        return Ok(await orderService.CreateOrderAsync(catalog.Store.Id, request, cancellationToken));
    }
}
