using System.Text.Json;

namespace Platform.Infrastructure.Services;

internal static class JsonGuards
{
    public static void EnsureValidJson(string? json, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            using var _ = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"{fieldName} must contain valid JSON. {ex.Message}");
        }
    }

    public static string NormalizeValueJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return "null";
        }

        EnsureValidJson(json, "Custom field value");
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement);
    }
}
