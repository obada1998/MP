namespace Platform.Application.Rendering;

public sealed record PageComponentDefinition(string Type, string Name, string Description);

public static class PageComponentCatalog
{
    private static readonly IReadOnlyDictionary<string, PageComponentDefinition> KnownComponents =
        new Dictionary<string, PageComponentDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["hero"] = new("hero", "Hero section", "Large intro section with headline, image, and primary action."),
            ["text"] = new("text", "Text block", "Rich text content for editorial sections."),
            ["image"] = new("image", "Image", "Responsive image with optional caption."),
            ["logo"] = new("logo", "Logo", "Store logo or custom logo image."),
            ["productGrid"] = new("productGrid", "Product grid", "Catalog grid filtered by category and limit."),
            ["categoryGrid"] = new("categoryGrid", "Category grid", "Category tiles for navigation."),
            ["button"] = new("button", "Button", "Standalone call-to-action button."),
            ["banner"] = new("banner", "Banner", "Promotional full-width banner."),
            ["featuredProducts"] = new("featuredProducts", "Featured products", "Curated product list."),
            ["navBar"] = new("navBar", "Navigation header", "Brand navigation with links and a primary action."),
            ["featureGrid"] = new("featureGrid", "Feature grid", "Structured feature, service, or highlight cards."),
            ["testimonials"] = new("testimonials", "Testimonials", "Customer quotes and credibility cards."),
            ["pricing"] = new("pricing", "Pricing", "Packages, plans, or service tiers."),
            ["footer"] = new("footer", "Footer", "Footer content with brand message and links."),
            ["container"] = new("container", "Container", "Section or boxed container that can hold nested elements."),
            ["columns"] = new("columns", "Columns", "Responsive multi-column layout block for grouped content."),
            ["spacer"] = new("spacer", "Spacer", "Vertical spacing block for layout rhythm."),
            ["input"] = new("input", "Input", "Form field element with label and placeholder."),
            ["form"] = new("form", "Form", "Simple lead-capture form container with nested inputs."),
            ["customHtml"] = new("customHtml", "Custom HTML", "Restricted custom markup block.")
        };

    public static IReadOnlyCollection<PageComponentDefinition> All => KnownComponents.Values.ToArray();

    public static bool IsKnown(string type) => KnownComponents.ContainsKey(type);

    public static PageComponentDefinition Get(string type)
    {
        if (KnownComponents.TryGetValue(type, out var definition))
        {
            return definition;
        }

        throw new KeyNotFoundException($"Unknown page component type '{type}'.");
    }
}
