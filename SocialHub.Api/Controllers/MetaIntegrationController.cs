using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialHub.Api.Configuration;
using SocialHub.Api.Data;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;
using SocialHub.Api.Services;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/integrations/meta")]
public sealed class MetaIntegrationController : ControllerBase
{
    private const string FrontendResultUrl = "http://localhost:3000/social-accounts";
    private readonly SocialHubDbContext _db;
    private readonly IMetaOAuthService _meta;
    private readonly IMetaOAuthStateStore _stateStore;
    private readonly MetaOptions _options;
    private readonly ILogger<MetaIntegrationController> _logger;

    public MetaIntegrationController(SocialHubDbContext db, IMetaOAuthService meta,
        IMetaOAuthStateStore stateStore, IOptions<MetaOptions> options, ILogger<MetaIntegrationController> logger)
    {
        _db = db;
        _meta = meta;
        _stateStore = stateStore;
        _options = options.Value;
        _logger = logger;
    }

    [HttpGet("connect")]
    public async Task<IActionResult> Connect([FromQuery] int workspaceId, CancellationToken cancellationToken)
    {
        if (workspaceId <= 0 || !await _db.Workspaces.AnyAsync(workspace => workspace.Id == workspaceId, cancellationToken))
            return NotFound(new { success = false, message = "Workspace was not found." });
        if (string.IsNullOrWhiteSpace(_options.AppId) || string.IsNullOrWhiteSpace(_options.AppSecret)
            || string.IsNullOrWhiteSpace(_options.RedirectUri))
            return Problem(statusCode: 503, title: "Meta integration is not configured.");

        var state = _stateStore.Create(workspaceId);
        return Redirect(_meta.CreateAuthorizationUrl(state));
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state,
        [FromQuery] string? error, [FromQuery(Name = "error_description")] string? errorDescription,
        CancellationToken cancellationToken)
    {
        if (!_stateStore.TryConsume(state ?? string.Empty, out var workspaceId))
            return FrontendError("invalid_state");

        if (!string.IsNullOrEmpty(error))
        {
            _logger.LogInformation("Meta authorization was not completed. Provider error: {Error}", SafeErrorCode(error));
            return FrontendError("authorization_denied");
        }
        if (string.IsNullOrWhiteSpace(code))
            return FrontendError("missing_code");

        try
        {
            var page = await _meta.ExchangeCodeAndGetFirstPageAsync(code, cancellationToken);
            var account = await _db.SocialAccounts.FirstOrDefaultAsync(item => item.WorkspaceId == workspaceId
                && item.Platform == SocialPlatforms.Facebook && item.PlatformAccountId == page.Id, cancellationToken);

            if (account is null)
            {
                account = new SocialAccount
                {
                    WorkspaceId = workspaceId,
                    Platform = SocialPlatforms.Facebook,
                    PlatformAccountId = page.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _db.SocialAccounts.Add(account);
            }

            var now = DateTime.UtcNow;
            account.Username = string.IsNullOrWhiteSpace(page.Username) ? page.Id : page.Username;
            account.DisplayName = page.Name;
            account.IsConnected = true;
            account.AccessToken = page.AccessToken;
            account.RefreshToken = null;
            account.TokenExpiresAt = page.TokenExpiresAt;
            account.ConnectedAt = now;
            account.UpdatedAt = now;
            await _db.SaveChangesAsync(cancellationToken);
            return Redirect($"{FrontendResultUrl}?facebook=connected");
        }
        catch (MetaOAuthException ex)
        {
            _logger.LogWarning("Meta OAuth connection failed: {Reason}", ex.Message);
            return FrontendError("meta_connection_failed");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Meta OAuth connection failed while saving or processing the account.");
            return FrontendError("connection_failed");
        }
    }

    private IActionResult FrontendError(string reason) => Redirect($"{FrontendResultUrl}?facebook=error&reason={Uri.EscapeDataString(reason)}");

    private static string SafeErrorCode(string error) => error.Length <= 80 && error.All(character => char.IsLetterOrDigit(character) || character is '_' or '-')
        ? error : "provider_error";
}
