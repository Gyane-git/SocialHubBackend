using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class SocialAccountService : ISocialAccountService
{
    private readonly SocialHubDbContext _context;

    public SocialAccountService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SocialAccountResponse>> GetAllAsync()
    {
        return await _context.SocialAccounts
            .AsNoTracking()
            .OrderBy(sa => sa.Platform)
            .ThenBy(sa => sa.Username)
            .Select(sa => MapToResponse(sa))
            .ToListAsync();
    }

    public async Task<SocialAccountResponse> GetByIdAsync(int id)
    {
        var account = await _context.SocialAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(sa => sa.Id == id);

        if (account == null)
        {
            throw new NotFoundException($"Social account with ID {id} was not found.");
        }

        return MapToResponse(account);
    }

    public async Task<IEnumerable<SocialAccountResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        return await _context.SocialAccounts
            .AsNoTracking()
            .Where(sa => sa.WorkspaceId == workspaceId)
            .OrderBy(sa => sa.Platform)
            .ThenBy(sa => sa.Username)
            .Select(sa => MapToResponse(sa))
            .ToListAsync();
    }

    public async Task<SocialAccountResponse> CreateAsync(CreateSocialAccountRequest request)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == request.WorkspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {request.WorkspaceId} was not found.");
        }

        if (!SocialPlatforms.IsValid(request.Platform))
        {
            throw new BadRequestException($"Invalid platform '{request.Platform}'. Supported platforms are: {string.Join(", ", SocialPlatforms.All)}.");
        }

        var duplicateExists = await _context.SocialAccounts.AnyAsync(sa =>
            sa.WorkspaceId == request.WorkspaceId &&
            sa.Platform == request.Platform &&
            sa.PlatformAccountId == request.PlatformAccountId);

        if (duplicateExists)
        {
            throw new ConflictException($"An account with ID '{request.PlatformAccountId}' on {request.Platform} already exists in this workspace.");
        }

        var account = new SocialAccount
        {
            WorkspaceId = request.WorkspaceId,
            Platform = request.Platform,
            PlatformAccountId = request.PlatformAccountId.Trim(),
            Username = request.Username.Trim(),
            DisplayName = request.DisplayName.Trim(),
            AvatarUrl = request.AvatarUrl?.Trim(),
            IsConnected = true,
            ConnectedAt = DateTime.UtcNow,
            AccessToken = $"mock_access_token_{Guid.NewGuid():N}",
            RefreshToken = $"mock_refresh_token_{Guid.NewGuid():N}",
            TokenExpiresAt = DateTime.UtcNow.AddDays(60),
            CreatedAt = DateTime.UtcNow
        };

        _context.SocialAccounts.Add(account);
        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    public async Task<SocialAccountResponse> UpdateAsync(int id, UpdateSocialAccountRequest request)
    {
        var account = await _context.SocialAccounts.FindAsync(id);
        if (account == null)
        {
            throw new NotFoundException($"Social account with ID {id} was not found.");
        }

        account.Username = request.Username.Trim();
        account.DisplayName = request.DisplayName.Trim();
        account.AvatarUrl = request.AvatarUrl?.Trim();
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    public async Task DeleteAsync(int id)
    {
        var account = await _context.SocialAccounts.FindAsync(id);
        if (account == null)
        {
            throw new NotFoundException($"Social account with ID {id} was not found.");
        }

        _context.SocialAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public async Task<SocialAccountResponse> ConnectAsync(int id)
    {
        var account = await _context.SocialAccounts.FindAsync(id);
        if (account == null)
        {
            throw new NotFoundException($"Social account with ID {id} was not found.");
        }

        account.IsConnected = true;
        account.ConnectedAt = DateTime.UtcNow;
        account.AccessToken ??= $"mock_access_token_{Guid.NewGuid():N}";
        account.RefreshToken ??= $"mock_refresh_token_{Guid.NewGuid():N}";
        account.TokenExpiresAt = DateTime.UtcNow.AddDays(60);
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    public async Task<SocialAccountResponse> DisconnectAsync(int id)
    {
        var account = await _context.SocialAccounts.FindAsync(id);
        if (account == null)
        {
            throw new NotFoundException($"Social account with ID {id} was not found.");
        }

        account.IsConnected = false;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(account);
    }

    private static SocialAccountResponse MapToResponse(SocialAccount sa)
    {
        return new SocialAccountResponse
        {
            Id = sa.Id,
            WorkspaceId = sa.WorkspaceId,
            Platform = sa.Platform,
            PlatformAccountId = sa.PlatformAccountId,
            Username = sa.Username,
            DisplayName = sa.DisplayName,
            AvatarUrl = sa.AvatarUrl,
            IsConnected = sa.IsConnected,
            TokenExpiresAt = sa.TokenExpiresAt,
            ConnectedAt = sa.ConnectedAt,
            CreatedAt = sa.CreatedAt,
            UpdatedAt = sa.UpdatedAt
        };
    }
}
