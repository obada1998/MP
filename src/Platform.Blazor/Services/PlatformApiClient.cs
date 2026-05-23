using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Blazor.Services;

public sealed class PlatformApiClient(HttpClient httpClient, BrowserTokenStore tokenStore)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Get, path, cancellationToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadResponseAsync<T>(response, cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Post, path, cancellationToken);
        request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Put, path, cancellationToken);
        request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Patch, path, cancellationToken);
        request.Content = JsonContent.Create(body, options: JsonOptions);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        return await ReadResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        using var request = await CreateRequestAsync(HttpMethod.Delete, path, cancellationToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task SetTokenAsync(string token)
    {
        await tokenStore.SetTokenAsync(token);
    }

    public async Task ClearTokenAsync()
    {
        await tokenStore.ClearTokenAsync();
    }

    public async Task<string?> TryGetTokenAsync()
    {
        return await tokenStore.GetTokenAsync();
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string path, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, path);
        var token = await TryGetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    private static async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);
        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ProblemDetails? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions, cancellationToken);
        }
        catch
        {
            // Fallback to status text below when the API did not return a problem payload.
        }

        throw new InvalidOperationException(problem?.Detail ?? $"{(int)response.StatusCode} {response.ReasonPhrase}");
    }
}
