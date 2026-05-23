using Platform.Application.Products;

namespace Platform.Blazor.Services;

public sealed class CartService
{
    private readonly List<CartItem> items = [];

    public IReadOnlyCollection<CartItem> Items => items;
    public int Count => items.Sum(x => x.Quantity);
    public decimal Total => items.Sum(x => x.LineTotal);

    public void Add(ProductDto product, int quantity = 1)
    {
        var existing = items.SingleOrDefault(x => x.Product.Id == product.Id);
        if (existing is null)
        {
            items.Add(new CartItem(product, Math.Max(1, quantity)));
            return;
        }

        existing.Quantity += Math.Max(1, quantity);
    }

    public void Remove(Guid productId)
    {
        items.RemoveAll(x => x.Product.Id == productId);
    }

    public void Clear() => items.Clear();
}

public sealed class CartItem(ProductDto product, int quantity)
{
    public ProductDto Product { get; } = product;
    public int Quantity { get; set; } = quantity;
    public decimal LineTotal => Product.BasePrice * Quantity;
}
