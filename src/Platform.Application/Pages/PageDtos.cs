using Platform.Domain.Enums;

namespace Platform.Application.Pages;

public sealed class PageDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
}

public sealed class UpsertPageRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
}

public sealed class PageLayoutDto
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public Guid StoreId { get; set; }
    public int Version { get; set; }
    public PageLayoutStatus Status { get; set; }
    public string LayoutJson { get; set; } = "{}";
    public PageLayoutDocument Layout { get; set; } = new();
    public DateTimeOffset? PublishedAt { get; set; }
}

public sealed class SavePageLayoutRequest
{
    public string LayoutJson { get; set; } = "{}";
    public bool Publish { get; set; }
}
