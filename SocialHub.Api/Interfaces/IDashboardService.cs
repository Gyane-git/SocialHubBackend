using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int workspaceId);
}
