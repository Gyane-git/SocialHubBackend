using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Get analytics snapshots for all social accounts in a workspace.
    /// </summary>
    [HttpGet("workspace/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AnalyticsSnapshotResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var snapshots = await _analyticsService.GetByWorkspaceIdAsync(workspaceId);
        return Ok(ApiResponse<IEnumerable<AnalyticsSnapshotResponse>>.Ok(snapshots, "Workspace analytics snapshots retrieved successfully."));
    }

    /// <summary>
    /// Get analytics snapshots for a specific social account.
    /// </summary>
    [HttpGet("social-account/{socialAccountId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AnalyticsSnapshotResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySocialAccount(int socialAccountId)
    {
        var snapshots = await _analyticsService.GetBySocialAccountIdAsync(socialAccountId);
        return Ok(ApiResponse<IEnumerable<AnalyticsSnapshotResponse>>.Ok(snapshots, "Social account analytics snapshots retrieved successfully."));
    }

    /// <summary>
    /// Get aggregated analytics summary for a workspace across all platforms.
    /// </summary>
    [HttpGet("summary/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<AnalyticsSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(int workspaceId)
    {
        var summary = await _analyticsService.GetSummaryAsync(workspaceId);
        return Ok(ApiResponse<AnalyticsSummaryResponse>.Ok(summary, "Analytics summary retrieved successfully."));
    }
}
