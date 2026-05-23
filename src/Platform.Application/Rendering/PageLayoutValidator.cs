using System.Text.Json;
using System.Text.Json.Nodes;
using Platform.Application.Pages;

namespace Platform.Application.Rendering;

public sealed class PageLayoutValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];
    public PageLayoutDocument? Layout { get; set; }
    public string NormalizedJson { get; set; } = "{}";
}

public sealed class PageLayoutValidator
{
    private const int MaxLayoutLength = 128_000;
    private const int MaxSections = 100;
    private const int MaxDepth = 6;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public PageLayoutValidationResult Validate(string layoutJson)
    {
        var result = new PageLayoutValidationResult();

        if (string.IsNullOrWhiteSpace(layoutJson))
        {
            result.Errors.Add("Layout JSON is required.");
            return result;
        }

        if (layoutJson.Length > MaxLayoutLength)
        {
            result.Errors.Add($"Layout JSON exceeds the {MaxLayoutLength} character limit.");
            return result;
        }

        PageLayoutDocument? layout;
        try
        {
            layout = JsonSerializer.Deserialize<PageLayoutDocument>(layoutJson, JsonOptions);
        }
        catch (JsonException ex)
        {
            result.Errors.Add($"Layout JSON is invalid: {ex.Message}");
            return result;
        }

        if (layout is null)
        {
            result.Errors.Add("Layout JSON did not produce a document.");
            return result;
        }

        layout.Sections ??= [];
        var sectionCount = CountSections(layout.Sections);
        if (sectionCount > MaxSections)
        {
            result.Errors.Add($"A page can contain at most {MaxSections} sections.");
        }

        ValidateSections(layout.Sections, result.Errors);

        layout.Sections = NormalizeSections(layout.Sections);
        result.Layout = layout;
        result.NormalizedJson = JsonSerializer.Serialize(layout, JsonOptions);
        return result;
    }

    private static void ValidateSections(
        List<PageSectionDefinition> sections,
        List<string> errors,
        int depth = 0,
        HashSet<string>? ids = null)
    {
        ids ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (depth > MaxDepth)
        {
            errors.Add($"Layout nesting cannot exceed {MaxDepth} levels.");
            return;
        }

        foreach (var section in sections)
        {
            if (string.IsNullOrWhiteSpace(section.Id))
            {
                section.Id = Guid.NewGuid().ToString("n");
            }

            if (!ids.Add(section.Id))
            {
                section.Id = Guid.NewGuid().ToString("n");
            }

            if (string.IsNullOrWhiteSpace(section.Type))
            {
                errors.Add("Every section must include a type.");
                continue;
            }

            if (!PageComponentCatalog.IsKnown(section.Type))
            {
                errors.Add($"Unknown page component type '{section.Type}'.");
            }

            section.Props ??= [];
            section.Styles ??= [];
            section.Children ??= [];

            if (section.Order < 0)
            {
                errors.Add($"Section '{section.Type}' has a negative order.");
            }

            if (section.Type.Equals("customHtml", StringComparison.OrdinalIgnoreCase) &&
                section.Props.TryGetPropertyValue("html", out var htmlNode) &&
                htmlNode is JsonValue htmlValue &&
                htmlValue.TryGetValue<string>(out var html) &&
                ContainsUnsafeMarkup(html))
            {
                errors.Add("Custom HTML cannot contain script tags, inline event handlers, or javascript: URLs.");
            }

            ValidateSections(section.Children, errors, depth + 1, ids);
        }
    }

    private static List<PageSectionDefinition> NormalizeSections(List<PageSectionDefinition> sections)
    {
        var ordered = sections.OrderBy(section => section.Order).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            ordered[i].Order = i + 1;
            ordered[i].Children ??= [];
            ordered[i].Children = NormalizeSections(ordered[i].Children);
        }

        return ordered;
    }

    private static int CountSections(IEnumerable<PageSectionDefinition> sections)
    {
        var count = 0;
        foreach (var section in sections)
        {
            count++;
            count += CountSections(section.Children ?? []);
        }

        return count;
    }

    private static bool ContainsUnsafeMarkup(string html)
    {
        return html.Contains("<script", StringComparison.OrdinalIgnoreCase) ||
               html.Contains("onerror=", StringComparison.OrdinalIgnoreCase) ||
               html.Contains("onclick=", StringComparison.OrdinalIgnoreCase) ||
               html.Contains("javascript:", StringComparison.OrdinalIgnoreCase);
    }
}
