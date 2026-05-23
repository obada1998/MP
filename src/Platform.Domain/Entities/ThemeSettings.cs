using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public sealed class ThemeSettings : Entity, IStoreScoped
{
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public string ThemeName { get; set; } = "Default";
    public string PrimaryColor { get; set; } = "#1f6feb";
    public string AccentColor { get; set; } = "#f97316";
    public string FontFamily { get; set; } = "Inter, system-ui, sans-serif";
    public string? CustomCss { get; set; }
    public string SettingsJson { get; set; } = "{}";
}
