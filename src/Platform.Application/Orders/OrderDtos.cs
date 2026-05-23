using Platform.Domain.Enums;

namespace Platform.Application.Orders;

public sealed class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public sealed class CreateOrderRequest
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public IReadOnlyCollection<CreateOrderItemRequest> Items { get; set; } = [];
}

public sealed class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public sealed class OrderDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Shipping { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyCollection<OrderItemDto> Items { get; set; } = [];
}

public sealed class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
