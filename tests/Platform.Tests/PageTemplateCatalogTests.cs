using System.Text.Json;
using Platform.Application.Pages;
using Platform.Application.Rendering;

namespace Platform.Tests;

public sealed class PageTemplateCatalogTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Built_in_templates_cover_required_categories()
    {
        var categories = PageTemplateCatalog.All.Select(template => template.Category).ToHashSet();

        foreach (var category in PageTemplateCatalog.Categories)
        {
            Assert.Contains(category, categories);
        }
    }

    [Fact]
    public void Built_in_templates_are_valid_layouts()
    {
        var validator = new PageLayoutValidator();

        foreach (var template in PageTemplateCatalog.All)
        {
            var layoutJson = JsonSerializer.Serialize(template.Layout, JsonOptions);
            var result = validator.Validate(layoutJson);

            Assert.True(result.IsValid, $"{template.Id}: {string.Join("; ", result.Errors)}");
        }
    }

    [Fact]
    public void Creating_template_layout_assigns_requested_page_id_and_fresh_ids()
    {
        var layout = PageTemplateCatalog.CreateLayout("fashion-boutique", "page-123");

        Assert.Equal("page-123", layout.PageId);
        Assert.NotEmpty(layout.Sections);
        Assert.Equal(CountSections(layout.Sections), layout.Sections.SelectMany(Flatten).Select(x => x.Id).Distinct().Count());
    }

    private static IEnumerable<PageSectionDefinition> Flatten(PageSectionDefinition section)
    {
        yield return section;
        foreach (var child in section.Children.SelectMany(Flatten))
        {
            yield return child;
        }
    }

    private static int CountSections(IEnumerable<PageSectionDefinition> sections)
    {
        return sections.Sum(section => 1 + CountSections(section.Children));
    }
}
