using SocialHub.Api.Models;

namespace SocialHub.Api.Interfaces;

public interface IMetaOAuthService
{
    string CreateAuthorizationUrl(string state);
    Task<MetaPage> ExchangeCodeAndGetFirstPageAsync(string code, CancellationToken cancellationToken = default);
}

public sealed record MetaPage(string Id, string Name, string AccessToken, string? Username = null, DateTime? TokenExpiresAt = null);
