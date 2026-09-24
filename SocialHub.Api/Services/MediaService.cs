using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class MediaService : IMediaService
{
    private readonly SocialHubDbContext _context;

    public MediaService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MediaResponse>> GetAllAsync()
    {
        return await _context.MediaItems
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => MapToResponse(m))
            .ToListAsync();
    }

    public async Task<MediaResponse> GetByIdAsync(int id)
    {
        var media = await _context.MediaItems
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (media == null)
        {
            throw new NotFoundException($"Media with ID {id} was not found.");
        }

        return MapToResponse(media);
    }

    public async Task<IEnumerable<MediaResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        return await _context.MediaItems
            .AsNoTracking()
            .Where(m => m.WorkspaceId == workspaceId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => MapToResponse(m))
            .ToListAsync();
    }

    public async Task<MediaResponse> CreateAsync(CreateMediaRequest request)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == request.WorkspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {request.WorkspaceId} was not found.");
        }

        var media = new Media
        {
            WorkspaceId = request.WorkspaceId,
            FileName = request.FileName.Trim(),
            FileUrl = request.FileUrl.Trim(),
            MimeType = request.MimeType.Trim(),
            FileSize = request.FileSize,
            Duration = request.Duration,
            CreatedAt = DateTime.UtcNow
        };

        _context.MediaItems.Add(media);
        await _context.SaveChangesAsync();

        return MapToResponse(media);
    }

    public async Task<MediaResponse> UpdateAsync(int id, UpdateMediaRequest request)
    {
        var media = await _context.MediaItems.FindAsync(id);
        if (media == null)
        {
            throw new NotFoundException($"Media with ID {id} was not found.");
        }

        media.FileName = request.FileName.Trim();
        media.FileUrl = request.FileUrl.Trim();
        media.MimeType = request.MimeType.Trim();
        media.FileSize = request.FileSize;
        media.Duration = request.Duration;

        await _context.SaveChangesAsync();

        return MapToResponse(media);
    }

    public async Task DeleteAsync(int id)
    {
        var media = await _context.MediaItems.FindAsync(id);
        if (media == null)
        {
            throw new NotFoundException($"Media with ID {id} was not found.");
        }

        // Disassociate posts using this media
        var referencingPosts = await _context.Posts.Where(p => p.MediaId == id).ToListAsync();
        foreach (var post in referencingPosts)
        {
            post.MediaId = null;
        }

        _context.MediaItems.Remove(media);
        await _context.SaveChangesAsync();
    }

    private static MediaResponse MapToResponse(Media m)
    {
        return new MediaResponse
        {
            Id = m.Id,
            WorkspaceId = m.WorkspaceId,
            FileName = m.FileName,
            FileUrl = m.FileUrl,
            MimeType = m.MimeType,
            FileSize = m.FileSize,
            Duration = m.Duration,
            CreatedAt = m.CreatedAt
        };
    }
}
