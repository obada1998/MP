using Microsoft.JSInterop;

namespace Platform.Blazor.Services;

public sealed class BrowserTokenStore(IJSRuntime jsRuntime)
{
    private const string TokenKey = "platform.jwt";
    private string? cachedToken;
    private bool initialized;

    public async Task<string?> GetTokenAsync()
    {
        if (initialized)
        {
            return cachedToken;
        }

        try
        {
            cachedToken = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch (InvalidOperationException)
        {
            cachedToken = null;
        }
        catch (JSException)
        {
            cachedToken = null;
        }

        initialized = true;
        return cachedToken;
    }

    public async Task SetTokenAsync(string token)
    {
        cachedToken = token;
        initialized = true;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }
        catch (InvalidOperationException)
        {
            // The in-memory token still protects navigation within the active Blazor circuit.
        }
        catch (JSException)
        {
            // The in-memory token still protects navigation within the active Blazor circuit.
        }
    }

    public async Task ClearTokenAsync()
    {
        cachedToken = null;
        initialized = true;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
        catch (InvalidOperationException)
        {
            // Token is already cleared for this circuit.
        }
        catch (JSException)
        {
            // Token is already cleared for this circuit.
        }
    }
}
