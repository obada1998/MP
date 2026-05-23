namespace Platform.Application.Rendering;

public sealed record PageComponentDefinition(string Type, string Name, string Description);

public static class PageComponentCatalog
{
    private static readonly IReadOnlyDictionary<string, PageComponentDefinition> KnownComponents =
        new Dictionary<string, PageComponentDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["hero"] = new("hero", "Hero section", "Large intro section with headline, image, and primary action."),
            ["textBlock"] = new("textBlock", "Text block", "Inline text content."),
            ["heading"] = new("heading", "Heading", "Headline text element."),
            ["paragraph"] = new("paragraph", "Paragraph", "Paragraph text content."),
            ["text"] = new("text", "Text block", "Rich text content for editorial sections."),
            ["image"] = new("image", "Image", "Responsive image with optional caption."),
            ["imageGallery"] = new("imageGallery", "Image gallery", "Gallery style image list."),
            ["videoEmbed"] = new("videoEmbed", "Video embed", "Embedded video element."),
            ["icon"] = new("icon", "Icon", "Icon or small vector symbol."),
            ["logo"] = new("logo", "Logo", "Store logo or custom logo image."),
            ["productGrid"] = new("productGrid", "Product grid", "Catalog grid filtered by category and limit."),
            ["productList"] = new("productList", "Product list", "List of products."),
            ["productCard"] = new("productCard", "Product card", "Single product card."),
            ["productDetails"] = new("productDetails", "Product details", "Detailed product content block."),
            ["categoryGrid"] = new("categoryGrid", "Category grid", "Category tiles for navigation."),
            ["categoryList"] = new("categoryList", "Category list", "Simple category list."),
            ["featuredProduct"] = new("featuredProduct", "Featured product", "Single featured product."),
            ["button"] = new("button", "Button", "Standalone call-to-action button."),
            ["addToCartButton"] = new("addToCartButton", "Add to cart button", "Commerce CTA for add to cart."),
            ["divider"] = new("divider", "Divider", "Horizontal divider line."),
            ["box"] = new("box", "Box container", "Simple boxed content container."),
            ["banner"] = new("banner", "Banner", "Promotional full-width banner."),
            ["header"] = new("header", "Header", "Page header section."),
            ["navigationMenu"] = new("navigationMenu", "Navigation menu", "Collection of navigation links."),
            ["featuredProducts"] = new("featuredProducts", "Featured products", "Curated product list."),
            ["navBar"] = new("navBar", "Navigation header", "Brand navigation with links and a primary action."),
            ["gridContainer"] = new("gridContainer", "Grid container", "Grid-based layout wrapper."),
            ["twoColumnSection"] = new("twoColumnSection", "Two column section", "Preset two-column section."),
            ["threeColumnSection"] = new("threeColumnSection", "Three column section", "Preset three-column section."),
            ["featureGrid"] = new("featureGrid", "Feature grid", "Structured feature, service, or highlight cards."),
            ["testimonials"] = new("testimonials", "Testimonials", "Customer quotes and credibility cards."),
            ["pricing"] = new("pricing", "Pricing", "Packages, plans, or service tiers."),
            ["footer"] = new("footer", "Footer", "Footer content with brand message and links."),
            ["container"] = new("container", "Container", "Section or boxed container that can hold nested elements."),
            ["columns"] = new("columns", "Columns", "Responsive multi-column layout block for grouped content."),
            ["spacer"] = new("spacer", "Spacer", "Vertical spacing block for layout rhythm."),
            ["input"] = new("input", "Input", "Form field element with label and placeholder."),
            ["form"] = new("form", "Form", "Simple lead-capture form container with nested inputs."),
            ["contactForm"] = new("contactForm", "Contact form", "Contact form with default fields."),
            ["newsletterForm"] = new("newsletterForm", "Newsletter form", "Newsletter signup form."),
            ["searchBox"] = new("searchBox", "Search box", "Search input with action."),
            ["customHtml"] = new("customHtml", "Custom HTML", "Restricted custom markup block.")
            ,["json"] = new("json", "JSON", "Render structured JSON data in a formatted code block.")
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
