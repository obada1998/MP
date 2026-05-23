using Platform.Application.Auth;

namespace Platform.Blazor.Services;

public sealed class AuthClient(
    PlatformApiClient apiClient,
    JwtAuthenticationStateProvider authenticationStateProvider)
{
    public AuthResponse? CurrentUser { get; private set; }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await apiClient.PostAsync<LoginRequest, AuthResponse>("api/auth/login", request)
            ?? throw new InvalidOperationException("Login failed.");
        await authenticationStateProvider.SignInAsync(response.Token);
        CurrentUser = response;
        return response;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await apiClient.PostAsync<RegisterRequest, AuthResponse>("api/auth/register", request)
            ?? throw new InvalidOperationException("Registration failed.");
        await authenticationStateProvider.SignInAsync(response.Token);
        CurrentUser = response;
        return response;
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        await authenticationStateProvider.SignOutAsync();
    }

    public async Task<bool> HasTokenAsync() => !string.IsNullOrWhiteSpace(await apiClient.TryGetTokenAsync());
}
