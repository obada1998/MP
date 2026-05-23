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
        Define("container", "Section", "A page section that can hold other elements.", "Layout", "layout"),
        Define("columns", "Columns", "A responsive multi-column layout block.", "Layout", "columns"),
        Define("spacer", "Spacer", "Vertical spacing between blocks.", "Layout", "space"),
        Define("hero", "Hero", "Headline, image, and call-to-action.", "Content", "hero"),
        Define("text", "Text", "Heading and paragraph copy.", "Content", "text"),
        Define("image", "Image", "Responsive image with caption.", "Content", "image"),
        Define("logo", "Logo", "Store logo or custom logo image.", "Content", "logo"),
        Define("button", "Button", "Clickable call-to-action.", "Content", "button"),
        Define("banner", "Banner", "Promotional banner strip.", "Content", "banner"),
        Define("navBar", "Navigation", "Brand, links, and primary action.", "Content", "nav"),
        Define("featureGrid", "Features", "Service, benefit, or highlight cards.", "Content", "cards"),
        Define("testimonials", "Testimonials", "Customer quotes and social proof.", "Content", "quote"),
        Define("pricing", "Pricing", "Packages, tiers, or service plans.", "Content", "price"),
        Define("footer", "Footer", "Brand footer with links.", "Content", "foot"),
        Define("productGrid", "Product grid", "Dynamic catalog grid.", "Commerce", "grid"),
        Define("categoryGrid", "Category grid", "Dynamic category grid.", "Commerce", "grid"),
        Define("featuredProducts", "Featured products", "Curated product list.", "Commerce", "star"),
        Define("form", "Form", "Lead-capture form container.", "Forms", "form"),
        Define("input", "Input", "Text, email, phone, or number field.", "Forms", "input"),
        Define("customHtml", "HTML", "Restricted custom markup.", "Advanced", "code")
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

        return All.SingleOrDefault(component => component.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
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
