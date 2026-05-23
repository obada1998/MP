using System.Text.Json.Nodes;

namespace Platform.Blazor.Rendering;

public static class JsonProps
{
    public static string GetString(JsonObject props, string name, string fallback = "")
    {
        return props.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue<string>(out var text)
            ? text
            : fallback;
    }

    public static int GetInt(JsonObject props, string name, int fallback)
    {
        return props.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue<int>(out var number)
            ? number
            : fallback;
    }

    public static bool GetBool(JsonObject props, string name, bool fallback = false)
    {
        return props.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue<bool>(out var boolean)
            ? boolean
            : fallback;
    }

    public static Guid? GetGuid(JsonObject props, string name)
    {
        var value = GetString(props, name);
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    public static IReadOnlyCollection<Guid> GetGuidArray(JsonObject props, string name)
    {
        if (!props.TryGetPropertyValue(name, out var node) || node is not JsonArray array)
        {
            return [];
        }

        var values = new List<Guid>();
        foreach (var item in array)
        {
            if (Guid.TryParse(item?.GetValue<string>(), out var value))
            {
                values.Add(value);
            }
        }

        return values;
    }

    public static IReadOnlyCollection<string> GetStringArray(JsonObject props, string name)
    {
        if (!props.TryGetPropertyValue(name, out var node) || node is not JsonArray array)
        {
            return [];
        }

        return array
            .Select(item => item is JsonValue value && value.TryGetValue<string>(out var text) ? text : null)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Cast<string>()
            .ToArray();
    }

    public static IReadOnlyCollection<JsonObject> GetObjectArray(JsonObject props, string name)
    {
        if (!props.TryGetPropertyValue(name, out var node) || node is not JsonArray array)
        {
            return [];
        }

        return array
            .OfType<JsonObject>()
            .ToArray();
    }
}
