using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Application.Orders;

namespace Platform.Api.Controllers;

[ApiController]
[Route("api/stores/{storeId:guid}/orders")]
[Authorize]
public sealed class OrdersController(IOrderService orderService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderDto>>> Get(Guid storeId, CancellationToken cancellationToken)
    {
        return Ok(await orderService.GetOrdersAsync(CurrentUserId, storeId, cancellationToken));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<OrderDto>> Create(Guid storeId, CreateOrderRequest request, CancellationToken cancellationToken)
    {
        return Ok(await orderService.CreateOrderAsync(storeId, request, cancellationToken));
    }

    [HttpPatch("{orderId:guid}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid storeId, Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return Ok(await orderService.UpdateStatusAsync(CurrentUserId, storeId, orderId, request, cancellationToken));
    }
}
