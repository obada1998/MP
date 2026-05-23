using System.Text.Json.Nodes;

namespace Platform.Application.Pages;

public sealed class PageLayoutDocument
{
    public string PageId { get; set; } = string.Empty;
    public List<PageSectionDefinition> Sections { get; set; } = [];
}

public sealed class PageSectionDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString("n");
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public JsonObject Props { get; set; } = [];
    public JsonObject Styles { get; set; } = [];
    public List<PageSectionDefinition> Children { get; set; } = [];
}
