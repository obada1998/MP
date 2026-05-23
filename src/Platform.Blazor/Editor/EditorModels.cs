using Platform.Application.Rendering;

namespace Platform.Blazor.Editor;

public sealed record EditorDropTarget(string? ParentId, int Index);

public sealed record EditorMoveRequest(string ElementId, int Offset);

public enum EditorZIndexChange
{
    Forward,
    Backward,
    Front,
    Back
}

public sealed record EditorZIndexRequest(string ElementId, EditorZIndexChange Change);

public sealed record EditorInlineEditRequest(string ElementId, string Key, string Value);

public enum EditorInteractionKind
{
    Move,
    Resize
}

public sealed record EditorInteractionRequest(
    string ElementId,
    EditorInteractionKind Kind,
    string? Handle,
    double ClientX,
    double ClientY,
    long PointerId,
    bool PreserveAspectRatio);

public sealed record EditorElementBox(
    string ElementId,
    double X,
    double Y,
    double Width,
    double Height,
    int ZIndex,
    bool IsVisible,
    bool IsLocked);

public sealed record EditorGuide(string Axis, double Position);

public sealed record EditorSnapResult(EditorElementBox Box, IReadOnlyList<EditorGuide> Guides);

public sealed class EditorCanvasRect
{
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public sealed record EditorComponentDefinition(
    string Type,
    string Name,
    string Description,
    string Group,
    string Icon);

public static class EditorComponentCatalog
{
    public static IReadOnlyCollection<EditorComponentDefinition> All { get; } =
    [
        Define("textBlock", "TextBlock", "Inline text content.", "Basic", "text"),
        Define("heading", "Heading", "Heading text with semantic level.", "Basic", "text"),
        Define("paragraph", "Paragraph", "Paragraph body copy.", "Basic", "text"),
        Define("button", "Button", "Clickable call-to-action.", "Basic", "button"),
        Define("divider", "Divider", "Horizontal dividing line.", "Basic", "layout"),
        Define("spacer", "Spacer", "Vertical spacing between blocks.", "Basic", "space"),
        Define("box", "Box/Container", "A boxed area that can hold child content.", "Basic", "layout"),
        Define("image", "Image", "Responsive image with caption.", "Media", "image"),
        Define("imageGallery", "ImageGallery", "Grid or carousel of images.", "Media", "image"),
        Define("videoEmbed", "VideoEmbed", "Embedded video by URL.", "Media", "image"),
        Define("icon", "Icon", "Small icon or symbol.", "Media", "logo"),
        Define("productGrid", "Product grid", "Dynamic catalog grid.", "Commerce", "grid"),
        Define("productCard", "ProductCard", "Single product preview card.", "Commerce", "grid"),
        Define("productDetails", "ProductDetails", "Detailed product content.", "Commerce", "grid"),
        Define("categoryGrid", "Category grid", "Dynamic category grid.", "Commerce", "grid"),
        Define("categoryList", "CategoryList", "List of product categories.", "Commerce", "grid"),
        Define("featuredProducts", "Featured products", "Curated product list.", "Commerce", "star"),
        Define("addToCartButton", "AddToCartButton", "Commerce add-to-cart action.", "Commerce", "button"),
        Define("header", "Header", "Top page header block.", "Layout", "nav"),
        Define("footer", "Footer", "Brand footer with links.", "Layout", "foot"),
        Define("navigationMenu", "NavigationMenu", "Menu with store links.", "Layout", "nav"),
        Define("gridContainer", "GridContainer", "Responsive grid layout wrapper.", "Layout", "columns"),
        Define("twoColumnSection", "TwoColumnSection", "Preset two-column layout.", "Layout", "columns"),
        Define("threeColumnSection", "ThreeColumnSection", "Preset three-column layout.", "Layout", "columns"),
        Define("container", "Section", "A page section that can hold other elements.", "Layout", "layout"),
        Define("columns", "Columns", "A responsive multi-column layout block.", "Layout", "columns"),
        Define("hero", "Hero", "Headline, image, and call-to-action.", "Layout", "hero"),
        Define("form", "ContactForm", "Lead-capture form container.", "Forms", "form"),
        Define("newsletterForm", "NewsletterForm", "Newsletter signup form.", "Forms", "form"),
        Define("searchBox", "SearchBox", "Search input and action.", "Forms", "input"),
        Define("form", "Form", "Lead-capture form container.", "Forms", "form"),
        Define("input", "Input", "Text, email, phone, or number field.", "Forms", "input"),
        Define("customHtml", "CustomHtmlSection", "Restricted custom markup.", "Custom", "code"),
        Define("navBar", "Navigation", "Brand, links, and primary action.", "Layout", "nav"),
        Define("featureGrid", "Features", "Service, benefit, or highlight cards.", "Layout", "cards"),
        Define("testimonials", "Testimonials", "Customer quotes and social proof.", "Layout", "quote"),
        Define("pricing", "Pricing", "Packages, tiers, or service plans.", "Layout", "price"),
        Define("banner", "Banner", "Promotional banner strip.", "Layout", "banner"),
        Define("logo", "Logo", "Store logo or custom logo image.", "Layout", "logo"),
        Define("text", "Text", "Heading and paragraph copy.", "Basic", "text")
    ];

    public static IReadOnlyCollection<string> Groups { get; } = All
        .Select(component => component.Group)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public static EditorComponentDefinition Get(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return new EditorComponentDefinition("unknown", "Unknown", string.Empty, "Advanced", "box");
        }

        return All.FirstOrDefault(component => component.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
            ?? new EditorComponentDefinition(type, PageComponentCatalog.Get(type).Name, string.Empty, "Advanced", "box");
    }

    public static bool CanContainChildren(string? type)
    {
        return string.Equals(type, "container", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(type, "columns", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(type, "form", StringComparison.OrdinalIgnoreCase);
    }

    private static EditorComponentDefinition Define(
        string type,
        string name,
        string description,
        string group,
        string icon)
    {
        return new EditorComponentDefinition(type, name, description, group, icon);
    }
}
