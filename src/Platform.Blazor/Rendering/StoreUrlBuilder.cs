namespace Platform.Blazor.Rendering;

public static class StoreUrlBuilder
{
    public static string Resolve(string? url, string storeKey)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return "#";
        }

        var trimmed = url.Trim();
        if (trimmed.StartsWith('#') ||
            trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (string.IsNullOrWhiteSpace(storeKey))
        {
            return trimmed;
        }

        var routeKey = trimmed.Trim('/').ToLowerInvariant();
        if (routeKey is "home" or "index")
        {
            return $"/store/{storeKey}";
        }

        if (routeKey is "shop" or "products" or "catalog" or "collections" or "new")
        {
            return $"/store/{storeKey}/products";
        }

        if (routeKey is "cart" or "bag")
        {
            return $"/store/{storeKey}/cart";
        }

        if (trimmed.StartsWith("/store/", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (trimmed == "/")
        {
            return $"/store/{storeKey}";
        }

        if (trimmed.StartsWith('/'))
        {
            return $"/store/{storeKey}{trimmed}";
        }

        return $"/store/{storeKey}/pages/{trimmed.Trim('/')}";
    }

    public static string FromLabel(string label, string storeKey)
    {
        return label.Trim().ToLowerInvariant() switch
        {
            "home" => Resolve("/", storeKey),
            "shop" or "products" or "catalog" or "collections" or "new" => Resolve("/products", storeKey),
            "cart" or "bag" => Resolve("/cart", storeKey),
            "contact" => "#contact",
            _ => Resolve(Slugify(label), storeKey)
        };
    }

    private static string Slugify(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray();
        var slug = new string(chars);
        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return slug.Trim('-');
    }
}
