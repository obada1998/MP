using Platform.Domain.Common;
using Platform.Domain.Enums;

namespace Platform.Domain.Entities;

public sealed class PageLayout : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Guid PageId { get; set; }
    public Page Page { get; set; } = null!;
    public int Version { get; set; }
    public PageLayoutStatus Status { get; set; } = PageLayoutStatus.Draft;
    public string LayoutJson { get; set; } = "{}";
    public DateTimeOffset? PublishedAt { get; set; }
}
