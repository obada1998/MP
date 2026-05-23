using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Application.Orders;
using Platform.Domain.Entities;
using Platform.Domain.Enums;
using Platform.Infrastructure.Data;

namespace Platform.Infrastructure.Services;

public sealed class OrderService(
    ApplicationDbContext dbContext,
    IStoreAccessService storeAccessService) : IOrderService
{
    private static readonly StoreMembershipRole[] OrderManagers = [StoreMembershipRole.StoreOwner, StoreMembershipRole.StoreStaff];

    public async Task<OrderDto> CreateOrderAsync(Guid storeId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var store = await dbContext.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.Id == storeId && x.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException("Store not found.");

        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("At least one order item is required.");
        }

        var requestedIds = request.Items.Select(x => x.ProductId).Distinct().ToArray();
        var products = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.StoreId == store.Id && x.Status == ProductStatus.Active && requestedIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (products.Count != requestedIds.Length)
        {
            throw new InvalidOperationException("One or more products are unavailable.");
        }

        var customer = await dbContext.Customers.SingleOrDefaultAsync(
            x => x.StoreId == storeId && x.Email == request.CustomerEmail,
            cancellationToken);
        if (customer is null)
        {
            customer = new Customer
            {
                StoreId = storeId,
                Email = request.CustomerEmail.Trim(),
                Name = request.CustomerName.Trim()
            };
            dbContext.Customers.Add(customer);
        }

        var order = new Order
        {
            StoreId = storeId,
            Customer = customer,
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerName = request.CustomerName.Trim(),
            OrderNumber = $"ORD-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            Status = OrderStatus.Pending
        };

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Item quantity must be greater than zero.");
            }

            var product = products[item.ProductId];
            var lineTotal = product.BasePrice * item.Quantity;
            order.Items.Add(new OrderItem
            {
                StoreId = storeId,
                ProductId = product.Id,
                ProductName = product.Name,
                Sku = product.Sku,
                UnitPrice = product.BasePrice,
                Quantity = item.Quantity,
                LineTotal = lineTotal
            });
        }

        order.Subtotal = order.Items.Sum(x => x.LineTotal);
        order.Tax = 0;
        order.Shipping = 0;
        order.Total = order.Subtotal + order.Tax + order.Shipping;

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (await QueryOrders(storeId).AsNoTracking().SingleAsync(x => x.Id == order.Id, cancellationToken)).ToDto();
    }

    public async Task<IReadOnlyCollection<OrderDto>> GetOrdersAsync(string userId, Guid storeId, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, OrderManagers, cancellationToken);
        var orders = await QueryOrders(storeId)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return orders.Select(x => x.ToDto()).ToArray();
    }

    public async Task<OrderDto> UpdateStatusAsync(string userId, Guid storeId, Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        await storeAccessService.EnsureStoreAccessAsync(userId, storeId, OrderManagers, cancellationToken);
        var order = await QueryOrders(storeId).SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");
        order.Status = request.Status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return order.ToDto();
    }

    private IQueryable<Order> QueryOrders(Guid storeId) => dbContext.Orders
        .Where(x => x.StoreId == storeId)
        .Include(x => x.Items);
}
