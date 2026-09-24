using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly SocialHubDbContext _context;

    public DashboardService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        // Post statistics
        var totalPosts = await _context.Posts.CountAsync(p => p.WorkspaceId == workspaceId);
        var publishedPosts = await _context.Posts.CountAsync(p => p.WorkspaceId == workspaceId && p.Status == PostStatuses.Published);
        var scheduledPosts = await _context.Posts.CountAsync(p => p.WorkspaceId == workspaceId && p.Status == PostStatuses.Scheduled);
        var failedPosts = await _context.Posts.CountAsync(p => p.WorkspaceId == workspaceId && p.Status == PostStatuses.Failed);

        // Connected accounts
        var connectedAccounts = await _context.SocialAccounts.CountAsync(sa => sa.WorkspaceId == workspaceId && sa.IsConnected);

        // Analytics aggregate
        var analyticsQuery = _context.AnalyticsSnapshots
            .AsNoTracking()
            .Where(a => a.SocialAccount.WorkspaceId == workspaceId);

        long totalReach = await analyticsQuery.SumAsync(a => (long)a.Reach);
        long totalImpressions = await analyticsQuery.SumAsync(a => (long)a.Impressions);
        long totalLikes = await analyticsQuery.SumAsync(a => (long)a.Likes);
        long totalComments = await analyticsQuery.SumAsync(a => (long)a.Comments);
        long totalShares = await analyticsQuery.SumAsync(a => (long)a.Shares);
        long totalEngagement = totalLikes + totalComments + totalShares;
        long totalViews = await analyticsQuery.SumAsync(a => (long)a.Views);

        // Recent posts (latest 5)
        var recentPostsEntities = await _context.Posts
            .AsNoTracking()
            .Where(p => p.WorkspaceId == workspaceId)
            .Include(p => p.Media)
            .Include(p => p.PostPlatforms)
            .Include(p => p.PostHashtags)
                .ThenInclude(ph => ph.Hashtag)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        var recentPosts = recentPostsEntities.Select(p => new PostResponse
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
        }).ToList();

        // Recent notifications (latest 5)
        var recentNotificationsEntities = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.WorkspaceId == workspaceId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var recentNotifications = recentNotificationsEntities.Select(n => new NotificationResponse
        {
            Id = n.Id,
            WorkspaceId = n.WorkspaceId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();

        return new DashboardSummaryResponse
        {
            WorkspaceId = workspaceId,
            TotalPosts = totalPosts,
            PublishedPosts = publishedPosts,
            ScheduledPosts = scheduledPosts,
            FailedPosts = failedPosts,
            ConnectedAccounts = connectedAccounts,
            TotalReach = totalReach,
            TotalImpressions = totalImpressions,
            TotalEngagement = totalEngagement,
            TotalViews = totalViews,
            RecentPosts = recentPosts,
            RecentNotifications = recentNotifications
        };
    }
}
