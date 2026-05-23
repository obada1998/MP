namespace Platform.Application.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid storeId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderDto>> GetOrdersAsync(string userId, Guid storeId, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateStatusAsync(string userId, Guid storeId, Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}
