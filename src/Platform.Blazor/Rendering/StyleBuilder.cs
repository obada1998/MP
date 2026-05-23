using System.Text;
using System.Text.Json.Nodes;

namespace Platform.Blazor.Rendering;

public static class StyleBuilder
{
    private static readonly HashSet<string> AllowedProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "background",
        "backgroundColor",
        "color",
        "position",
        "left",
        "top",
        "right",
        "bottom",
        "width",
        "maxWidth",
        "minHeight",
        "height",
        "zIndex",
        "visibility",
        "overflow",
        "opacity",
        "padding",
        "margin",
        "textAlign",
        "borderRadius",
        "border",
        "boxShadow",
        "display",
        "gap",
        "alignItems",
        "justifyContent",
        "fontSize",
        "fontFamily",
        "fontWeight",
        "lineHeight",
        "objectFit"
    };

    private static readonly HashSet<string> LayoutProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "position",
        "left",
        "top",
        "right",
        "bottom",
        "width",
        "maxWidth",
        "minHeight",
        "height",
        "zIndex",
        "visibility",
        "overflow"
    };

    public static string FromJson(JsonObject styles, bool includeLayout = true)
    {
        var builder = new StringBuilder();
        foreach (var (key, node) in styles)
        {
            if (!AllowedProperties.Contains(key) ||
                (!includeLayout && LayoutProperties.Contains(key)) ||
                node is not JsonValue value ||
                !value.TryGetValue<string>(out var raw))
            {
                continue;
            }

            var sanitized = Sanitize(raw);
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                continue;
            }

            builder.Append(ToKebabCase(key));
            builder.Append(':');
            builder.Append(sanitized);
            builder.Append(';');
        }

        return builder.ToString();
    }

    private static string Sanitize(string value)
    {
        if (value.Contains("expression", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("javascript:", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("<", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        return value.Length > 160 ? value[..160] : value;
    }

    private static string ToKebabCase(string value)
    {
        var builder = new StringBuilder(value.Length + 4);
        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];
            if (char.IsUpper(character))
            {
                if (i > 0)
                {
                    builder.Append('-');
                }

                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
