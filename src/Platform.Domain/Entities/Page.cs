using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class Page : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public ICollection<PageLayout> Layouts { get; set; } = new List<PageLayout>();
}
