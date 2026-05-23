using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Platform.Blazor.Services;

public sealed class JwtAuthenticationStateProvider(BrowserTokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync();
        return new AuthenticationState(CreatePrincipal(token));
    }

    public async Task SignInAsync(string token)
    {
        await tokenStore.SetTokenAsync(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CreatePrincipal(token))));
    }

    public async Task SignOutAsync()
    {
        await tokenStore.ClearTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private static ClaimsPrincipal CreatePrincipal(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anonymous;
        }

        try
        {
            var payload = DecodePayload(token);
            if (IsExpired(payload))
            {
                return Anonymous;
            }

            var claims = ReadClaims(payload).ToList();
            if (claims.Count == 0)
            {
                return Anonymous;
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
        }
        catch
        {
            return Anonymous;
        }
    }

    private static JsonElement DecodePayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("Invalid JWT format.");
        }

        var json = Encoding.UTF8.GetString(DecodeBase64Url(parts[1]));
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    private static IEnumerable<Claim> ReadClaims(JsonElement payload)
    {
        foreach (var property in payload.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in property.Value.EnumerateArray())
                {
                    var value = item.ValueKind == JsonValueKind.String ? item.GetString() : item.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        yield return new Claim(MapClaimType(property.Name), value);
                    }
                }

                continue;
            }

            var scalar = property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString()
                : property.Value.ToString();
            if (!string.IsNullOrWhiteSpace(scalar))
            {
                yield return new Claim(MapClaimType(property.Name), scalar);
            }
        }
    }

    private static bool IsExpired(JsonElement payload)
    {
        return payload.TryGetProperty("exp", out var exp)
            && exp.TryGetInt64(out var seconds)
            && DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow;
    }

    private static string MapClaimType(string claimType)
    {
        return claimType switch
        {
            "sub" or "nameid" => ClaimTypes.NameIdentifier,
            "email" => ClaimTypes.Email,
            "name" or "unique_name" => ClaimTypes.Name,
            "role" or "roles" => ClaimTypes.Role,
            _ => claimType
        };
    }

    private static byte[] DecodeBase64Url(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}
