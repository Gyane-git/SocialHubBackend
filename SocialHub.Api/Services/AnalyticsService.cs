using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly SocialHubDbContext _context;

    public AnalyticsService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AnalyticsSnapshotResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        var snapshots = await _context.AnalyticsSnapshots
            .AsNoTracking()
            .Include(a => a.SocialAccount)
            .Where(a => a.SocialAccount.WorkspaceId == workspaceId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return snapshots.Select(s => MapToResponse(s));
    }

    public async Task<IEnumerable<AnalyticsSnapshotResponse>> GetBySocialAccountIdAsync(int socialAccountId)
    {
        var accountExists = await _context.SocialAccounts.AnyAsync(sa => sa.Id == socialAccountId);
        if (!accountExists)
        {
            throw new NotFoundException($"Social account with ID {socialAccountId} was not found.");
        }

        var snapshots = await _context.AnalyticsSnapshots
            .AsNoTracking()
            .Include(a => a.SocialAccount)
            .Where(a => a.SocialAccountId == socialAccountId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return snapshots.Select(s => MapToResponse(s));
    }

    public async Task<AnalyticsSummaryResponse> GetSummaryAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        var snapshots = await _context.AnalyticsSnapshots
            .AsNoTracking()
            .Include(a => a.SocialAccount)
            .Where(a => a.SocialAccount.WorkspaceId == workspaceId)
            .ToListAsync();

        var accounts = await _context.SocialAccounts
            .AsNoTracking()
            .Where(sa => sa.WorkspaceId == workspaceId)
            .ToListAsync();

        // Calculate latest followers per account
        int totalFollowers = 0;
        foreach (var account in accounts)
        {
            var latestSnapshot = snapshots
                .Where(s => s.SocialAccountId == account.Id)
                .OrderByDescending(s => s.Date)
                .FirstOrDefault();

            if (latestSnapshot != null)
            {
                totalFollowers += latestSnapshot.Followers;
            }
        }

        long totalReach = snapshots.Sum(s => (long)s.Reach);
        long totalImpressions = snapshots.Sum(s => (long)s.Impressions);
        long totalLikes = snapshots.Sum(s => (long)s.Likes);
        long totalComments = snapshots.Sum(s => (long)s.Comments);
        long totalShares = snapshots.Sum(s => (long)s.Shares);
        long totalEngagement = totalLikes + totalComments + totalShares;
        long totalViews = snapshots.Sum(s => (long)s.Views);
        double avgEngagementRate = snapshots.Count > 0
            ? Math.Round(snapshots.Average(s => s.EngagementRate), 2)
            : 0;

        var platformMetrics = snapshots
            .GroupBy(s => s.SocialAccount.Platform)
            .Select(g =>
            {
                var platformAccounts = accounts.Where(a => a.Platform == g.Key).Select(a => a.Id).ToList();
                int platformFollowers = 0;
                foreach (var accId in platformAccounts)
                {
                    var latest = g.Where(s => s.SocialAccountId == accId).OrderByDescending(s => s.Date).FirstOrDefault();
                    if (latest != null) platformFollowers += latest.Followers;
                }

                long pLikes = g.Sum(s => (long)s.Likes);
                long pComments = g.Sum(s => (long)s.Comments);
                long pShares = g.Sum(s => (long)s.Shares);

                return new PlatformMetricsResponse
                {
                    Platform = g.Key,
                    Followers = platformFollowers,
                    Reach = g.Sum(s => (long)s.Reach),
                    Impressions = g.Sum(s => (long)s.Impressions),
                    Likes = pLikes,
                    Comments = pComments,
                    Shares = pShares,
                    Engagement = pLikes + pComments + pShares,
                    Views = g.Sum(s => (long)s.Views),
                    AverageEngagementRate = Math.Round(g.Average(s => s.EngagementRate), 2)
                };
            })
            .ToList();

        return new AnalyticsSummaryResponse
        {
            WorkspaceId = workspaceId,
            TotalFollowers = totalFollowers,
            TotalReach = totalReach,
            TotalImpressions = totalImpressions,
            TotalLikes = totalLikes,
            TotalComments = totalComments,
            TotalShares = totalShares,
            TotalEngagement = totalEngagement,
            TotalViews = totalViews,
            AverageEngagementRate = avgEngagementRate,
            ByPlatform = platformMetrics
        };
    }

    private static AnalyticsSnapshotResponse MapToResponse(AnalyticsSnapshot s)
    {
        return new AnalyticsSnapshotResponse
        {
            Id = s.Id,
            SocialAccountId = s.SocialAccountId,
            Platform = s.SocialAccount?.Platform,
            Username = s.SocialAccount?.Username,
            Date = s.Date,
            Followers = s.Followers,
            Reach = s.Reach,
            Impressions = s.Impressions,
            Likes = s.Likes,
            Comments = s.Comments,
            Shares = s.Shares,
            Views = s.Views,
            EngagementRate = s.EngagementRate,
            CreatedAt = s.CreatedAt
        };
    }
}
