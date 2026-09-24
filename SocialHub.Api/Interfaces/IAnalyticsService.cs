using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IAnalyticsService
{
    Task<IEnumerable<AnalyticsSnapshotResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<IEnumerable<AnalyticsSnapshotResponse>> GetBySocialAccountIdAsync(int socialAccountId);
    Task<AnalyticsSummaryResponse> GetSummaryAsync(int workspaceId);
}
