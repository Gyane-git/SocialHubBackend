using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SocialHub.Api.Configuration;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Services;

public sealed class MetaOAuthService : IMetaOAuthService
{
    private const string GraphBaseUrl = "https://graph.facebook.com/";
    private readonly HttpClient _httpClient;
    private readonly MetaOptions _options;

    public MetaOAuthService(HttpClient httpClient, IOptions<MetaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public string CreateAuthorizationUrl(string state)
    {
        var version = NormalizeVersion(_options.GraphApiVersion);
        var query = new Dictionary<string, string>
        {
            ["client_id"] = _options.AppId,
            ["redirect_uri"] = _options.RedirectUri,
            ["state"] = state,
            ["response_type"] = "code"
        };

        var scopes = _options.Scopes
            .Where(scope => !string.IsNullOrWhiteSpace(scope))
            .Select(scope => scope.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (scopes.Length > 0)
            query["scope"] = string.Join(',', scopes);

        return $"https://www.facebook.com/{version}/dialog/oauth?{string.Join("&", query.Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"))}";
    }

    public async Task<MetaPage> ExchangeCodeAndGetFirstPageAsync(string code, CancellationToken cancellationToken = default)
    {
        var version = NormalizeVersion(_options.GraphApiVersion);
        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{GraphBaseUrl}{version}/oauth/access_token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _options.AppId,
                ["client_secret"] = _options.AppSecret,
                ["redirect_uri"] = _options.RedirectUri,
                ["code"] = code
            })
        };

        using var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        var tokenBody = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
            throw new MetaOAuthException("Meta could not complete the authorization code exchange.");

        var token = JsonSerializer.Deserialize<TokenResponse>(tokenBody);
        if (string.IsNullOrWhiteSpace(token?.AccessToken))
            throw new MetaOAuthException("Meta returned an incomplete authorization response.");

        using var pageRequest = new HttpRequestMessage(HttpMethod.Get,
            $"{GraphBaseUrl}{version}/me/accounts?fields=id,name,access_token,username,category,category_list,tasks,picture");
        pageRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        using var pageResponse = await _httpClient.SendAsync(pageRequest, cancellationToken);
        var pageBody = await pageResponse.Content.ReadAsStringAsync(cancellationToken);
        if (!pageResponse.IsSuccessStatusCode)
            throw new MetaOAuthException("Meta could not retrieve Pages available to this account.");

        var pages = JsonSerializer.Deserialize<PageListResponse>(pageBody);
        var page = pages?.Data?.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Id)
            && !string.IsNullOrWhiteSpace(item.Name) && !string.IsNullOrWhiteSpace(item.AccessToken));
        if (page is null)
            throw new MetaOAuthException("No Facebook Pages are available to the authorized account.");

        // The code-exchange expiry belongs to the user token; it is not a reliable
        // expiry for the separately returned Page token, so leave it unknown here.
        return new MetaPage(page.Id!, page.Name!, page.AccessToken!, page.Username);
    }

    private static string NormalizeVersion(string version) => version.StartsWith('v') ? version : $"v{version}";

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

    }

    private sealed class PageListResponse
    {
        [JsonPropertyName("data")]
        public List<PageItem>? Data { get; set; }
    }

    private sealed class PageItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}

public sealed class MetaOAuthException : Exception
{
    public MetaOAuthException(string message) : base(message) { }
}
