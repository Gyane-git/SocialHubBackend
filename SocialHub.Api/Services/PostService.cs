using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class PostService : IPostService
{
    private readonly SocialHubDbContext _context;
    private readonly INotificationService _notificationService;

    public PostService(SocialHubDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<PostResponse>> GetAllAsync(PostQueryParameters parameters)
    {
        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Media)
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
                .ThenInclude(ph => ph.Hashtag)
            .AsQueryable();

        if (parameters.WorkspaceId.HasValue)
        {
            query = query.Where(p => p.WorkspaceId == parameters.WorkspaceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            var status = parameters.Status.Trim().ToLowerInvariant();
            query = query.Where(p => p.Status.ToLower() == status);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Platform))
        {
            var platform = parameters.Platform.Trim().ToLowerInvariant();
            query = query.Where(p => p.PostPlatforms.Any(pp => pp.Platform.ToLower() == platform));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLowerInvariant();
            query = query.Where(p => p.Title.ToLower().Contains(search) || p.Caption.ToLower().Contains(search));
        }

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => MapToResponse(p));
    }

    public async Task<PostResponse> GetByIdAsync(int id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Media)
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
                .ThenInclude(ph => ph.Hashtag)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        return MapToResponse(post);
    }

    public async Task<IEnumerable<PostResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        var posts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.WorkspaceId == workspaceId)
            .Include(p => p.Media)
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
                .ThenInclude(ph => ph.Hashtag)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => MapToResponse(p));
    }

    public async Task<PostResponse> CreateAsync(CreatePostRequest request)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == request.WorkspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {request.WorkspaceId} was not found.");
        }

        if (request.MediaId.HasValue)
        {
            var mediaExists = await _context.MediaItems.AnyAsync(m => m.Id == request.MediaId.Value && m.WorkspaceId == request.WorkspaceId);
            if (!mediaExists)
            {
                throw new NotFoundException($"Media with ID {request.MediaId.Value} was not found in this workspace.");
            }
        }

        var initialStatus = PostStatuses.Draft;
        if (!string.IsNullOrWhiteSpace(request.Status) && PostStatuses.IsValid(request.Status))
        {
            initialStatus = request.Status;
        }

        if (request.ScheduledAt.HasValue)
        {
            initialStatus = PostStatuses.Scheduled;
        }

        var post = new Post
        {
            WorkspaceId = request.WorkspaceId,
            Title = request.Title.Trim(),
            Caption = request.Caption.Trim(),
            Status = initialStatus,
            MediaId = request.MediaId,
            ScheduledAt = request.ScheduledAt,
            CreatedAt = DateTime.UtcNow
        };

        // Attach social account platforms
        if (request.SocialAccountIds.Count > 0)
        {
            var distinctAccountIds = request.SocialAccountIds.Distinct().ToList();
            var socialAccounts = await _context.SocialAccounts
                .Where(sa => sa.WorkspaceId == request.WorkspaceId && distinctAccountIds.Contains(sa.Id))
                .ToListAsync();

            foreach (var sa in socialAccounts)
            {
                post.PostPlatforms.Add(new PostPlatform
                {
                    SocialAccountId = sa.Id,
                    Platform = sa.Platform,
                    Status = initialStatus == PostStatuses.Scheduled ? PlatformPostStatuses.Pending : PlatformPostStatuses.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // Attach hashtags
        if (request.Hashtags.Count > 0)
        {
            await AttachHashtagsAsync(post, request.Hashtags);
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        return await GetByIdAsync(post.Id);
    }

    public async Task<PostResponse> UpdateAsync(int id, UpdatePostRequest request)
    {
        var post = await _context.Posts
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        if (request.MediaId.HasValue)
        {
            var mediaExists = await _context.MediaItems.AnyAsync(m => m.Id == request.MediaId.Value && m.WorkspaceId == post.WorkspaceId);
            if (!mediaExists)
            {
                throw new NotFoundException($"Media with ID {request.MediaId.Value} was not found in this workspace.");
            }
            post.MediaId = request.MediaId.Value;
        }
        else
        {
            post.MediaId = null;
        }

        post.Title = request.Title.Trim();
        post.Caption = request.Caption.Trim();
        post.ScheduledAt = request.ScheduledAt;
        post.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Status) && PostStatuses.IsValid(request.Status))
        {
            post.Status = request.Status;
        }

        if (request.SocialAccountIds != null)
        {
            // Remove existing platform assignments
            _context.PostPlatforms.RemoveRange(post.PostPlatforms);

            var distinctAccountIds = request.SocialAccountIds.Distinct().ToList();
            var socialAccounts = await _context.SocialAccounts
                .Where(sa => sa.WorkspaceId == post.WorkspaceId && distinctAccountIds.Contains(sa.Id))
                .ToListAsync();

            foreach (var sa in socialAccounts)
            {
                post.PostPlatforms.Add(new PostPlatform
                {
                    PostId = post.Id,
                    SocialAccountId = sa.Id,
                    Platform = sa.Platform,
                    Status = PlatformPostStatuses.Pending,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (request.Hashtags != null)
        {
            _context.PostHashtags.RemoveRange(post.PostHashtags);
            await AttachHashtagsAsync(post, request.Hashtags);
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(post.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task<PostResponse> PublishAsync(int id)
    {
        var post = await _context.Posts
            .Include(p => p.PostPlatforms)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        // 1. Change Post status to Processing
        post.Status = PostStatuses.Processing;
        foreach (var pp in post.PostPlatforms)
        {
            pp.Status = PlatformPostStatuses.Processing;
        }
        await _context.SaveChangesAsync();

        // 2 & 3 & 4. Simulate successful publishing with mock external post IDs
        var now = DateTime.UtcNow;
        foreach (var pp in post.PostPlatforms)
        {
            pp.Status = PlatformPostStatuses.Published;
            pp.ExternalPostId = $"mock_{pp.Platform.ToLowerInvariant()}_{Guid.NewGuid().ToString("N")[..12]}";
            pp.ErrorMessage = null;
            pp.PublishedAt = now;
            pp.UpdatedAt = now;
        }

        // 5 & 6. Change Post statuses to Published
        post.Status = PostStatuses.Published;
        post.PublishedAt = now;
        post.UpdatedAt = now;

        await _context.SaveChangesAsync();

        // Create success notification
        await _notificationService.CreateNotificationAsync(
            post.WorkspaceId,
            "Post Published",
            $"Post '{post.Title}' was published successfully to {post.PostPlatforms.Count} platform(s).",
            NotificationTypes.Success);

        return await GetByIdAsync(post.Id);
    }

    public async Task<PostResponse> ScheduleAsync(int id, SchedulePostRequest request)
    {
        var post = await _context.Posts
            .Include(p => p.PostPlatforms)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        post.Status = PostStatuses.Scheduled;
        post.ScheduledAt = request.ScheduledAt;
        post.UpdatedAt = DateTime.UtcNow;

        foreach (var pp in post.PostPlatforms)
        {
            pp.Status = PlatformPostStatuses.Pending;
            pp.PublishedAt = null;
            pp.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            post.WorkspaceId,
            "Post Scheduled",
            $"Post '{post.Title}' has been scheduled for {request.ScheduledAt:yyyy-MM-dd HH:mm} UTC.",
            NotificationTypes.Info);

        return await GetByIdAsync(post.Id);
    }

    public async Task<PostResponse> DuplicateAsync(int id)
    {
        var original = await _context.Posts
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (original == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        var copy = new Post
        {
            WorkspaceId = original.WorkspaceId,
            Title = $"[Copy] {original.Title}",
            Caption = original.Caption,
            Status = PostStatuses.Draft,
            MediaId = original.MediaId,
            ScheduledAt = null,
            PublishedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var pp in original.PostPlatforms)
        {
            copy.PostPlatforms.Add(new PostPlatform
            {
                SocialAccountId = pp.SocialAccountId,
                Platform = pp.Platform,
                Status = PlatformPostStatuses.Pending,
                ExternalPostId = null,
                ErrorMessage = null,
                PublishedAt = null,
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var ph in original.PostHashtags)
        {
            copy.PostHashtags.Add(new PostHashtag
            {
                HashtagId = ph.HashtagId
            });
        }

        _context.Posts.Add(copy);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(copy.Id);
    }

    public async Task<PostResponse> RetryAsync(int id)
    {
        var post = await _context.Posts
            .Include(p => p.PostPlatforms)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new NotFoundException($"Post with ID {id} was not found.");
        }

        // Reset error messages and re-trigger publish
        foreach (var pp in post.PostPlatforms)
        {
            pp.ErrorMessage = null;
        }

        return await PublishAsync(id);
    }

    private async Task AttachHashtagsAsync(Post post, IEnumerable<string> hashtags)
    {
        foreach (var tag in hashtags)
        {
            var raw = tag.Trim();
            if (string.IsNullOrWhiteSpace(raw)) continue;

            var normalized = raw.StartsWith('#') ? raw.ToLowerInvariant() : "#" + raw.ToLowerInvariant();
            var hashtag = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == normalized);
            if (hashtag == null)
            {
                hashtag = new Hashtag
                {
                    Name = normalized,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Hashtags.Add(hashtag);
                await _context.SaveChangesAsync();
            }

            if (!post.PostHashtags.Any(ph => ph.HashtagId == hashtag.Id))
            {
                post.PostHashtags.Add(new PostHashtag
                {
                    Post = post,
                    HashtagId = hashtag.Id
                });
            }
        }
    }

    private static PostResponse MapToResponse(Post p)
    {
        return new PostResponse
        {
            Id = p.Id,
            WorkspaceId = p.WorkspaceId,
            Title = p.Title,
            Caption = p.Caption,
            Status = p.Status,
            MediaId = p.MediaId,
            Media = p.Media != null ? new MediaResponse
            {
                Id = p.Media.Id,
                WorkspaceId = p.Media.WorkspaceId,
                FileName = p.Media.FileName,
                FileUrl = p.Media.FileUrl,
                MimeType = p.Media.MimeType,
                FileSize = p.Media.FileSize,
                Duration = p.Media.Duration,
                CreatedAt = p.Media.CreatedAt
            } : null,
            PublishedAt = p.PublishedAt,
            ScheduledAt = p.ScheduledAt,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Platforms = p.PostPlatforms.Select(pp => new PostPlatformResponse
            {
                Id = pp.Id,
                PostId = pp.PostId,
                SocialAccountId = pp.SocialAccountId,
                Platform = pp.Platform,
                Status = pp.Status,
                ExternalPostId = pp.ExternalPostId,
                ErrorMessage = pp.ErrorMessage,
                PublishedAt = pp.PublishedAt,
                CreatedAt = pp.CreatedAt,
                UpdatedAt = pp.UpdatedAt
            }).ToList(),
            Hashtags = p.PostHashtags.Select(ph => ph.Hashtag?.Name ?? string.Empty).Where(n => !string.IsNullOrEmpty(n)).ToList()
        };
    }
}
