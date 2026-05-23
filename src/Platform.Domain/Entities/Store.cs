using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class Store : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Domain { get; set; }
    public string ThemeName { get; set; } = "Default";
    public bool IsActive { get; set; } = true;

    public StoreSettings? Settings { get; set; }
    public ThemeSettings? ThemeSettings { get; set; }
    public ICollection<StoreUser> Users { get; set; } = new List<StoreUser>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Page> Pages { get; set; } = new List<Page>();
}
