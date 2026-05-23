using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Platform.Blazor.Services;

/// <summary>
/// Provides ASP.NET Core authentication services for Blazor server endpoints while
/// leaving component-level identity resolution to <see cref="JwtAuthenticationStateProvider"/>.
/// </summary>
public sealed class BlazorClientAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
