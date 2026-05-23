using Platform.Domain.Enums;

namespace Platform.Application.Stores;

public sealed class StoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Domain { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public StoreMembershipRole? CurrentUserRole { get; set; }
}

public sealed class CreateStoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? LogoUrl { get; set; }
    public string? Domain { get; set; }
    public string ThemeName { get; set; } = "Default";
    public IReadOnlyCollection<string> DefaultPages { get; set; } = [];
}

public sealed class UpdateStoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Domain { get; set; }
    public string ThemeName { get; set; } = "Default";
    public bool IsActive { get; set; }
}

public sealed class StoreSettingsDto
{
    public Guid StoreId { get; set; }
    public string Currency { get; set; } = "USD";
    public string Culture { get; set; } = "en-US";
    public string? ContactEmail { get; set; }
    public string SettingsJson { get; set; } = "{}";
}

public sealed class ThemeSettingsDto
{
    public Guid StoreId { get; set; }
    public string ThemeName { get; set; } = "Default";
    public string PrimaryColor { get; set; } = "#1f6feb";
    public string AccentColor { get; set; } = "#f97316";
    public string FontFamily { get; set; } = "Inter, system-ui, sans-serif";
    public string? CustomCss { get; set; }
    public string SettingsJson { get; set; } = "{}";
}
