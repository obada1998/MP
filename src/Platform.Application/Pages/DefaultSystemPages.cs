namespace Platform.Application.Pages;

public sealed record DefaultSystemPageDefinition(
    string Key,
    string Title,
    string Slug,
    string TemplateId,
    bool IsHomePage = false,
    bool IsPublished = true);

public static class DefaultSystemPages
{
    public const string Home = "home";
    public const string Products = "products";
    public const string ProductDetails = "product-details";
    public const string Contact = "contact";
    public const string AboutUs = "about-us";
    public const string Login = "login";
    public const string Cart = "cart";
    public const string Checkout = "checkout";

    public static IReadOnlyCollection<DefaultSystemPageDefinition> All { get; } =
    [
        new(Home, "Home Page", "home", "fashion-boutique", IsHomePage: true),
        new(Products, "Products Page", "products", "electronics-store"),
        new(ProductDetails, "Product Details Page", "product-details", "digital-products"),
        new(Contact, "Contact Page", "contact", "blank-page"),
        new(AboutUs, "About Us Page", "about-us", "blank-page"),
        new(Login, "Login Page", "login", "blank-page"),
        new(Cart, "Cart Page", "cart", "blank-page"),
        new(Checkout, "Checkout Page", "checkout", "blank-page")
    ];

    public static IReadOnlyCollection<string> AllKeys { get; } = All.Select(x => x.Key).ToArray();

    public static IReadOnlyCollection<DefaultSystemPageDefinition> Resolve(IReadOnlyCollection<string>? keys)
    {
        if (keys is null || keys.Count == 0)
        {
            return All;
        }

        var selected = keys
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return All.Where(x => selected.Contains(x.Key)).ToArray();
    }
}
