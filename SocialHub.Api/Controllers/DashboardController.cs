using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Get dashboard summary metrics and recent activity for a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <returns>Dashboard summary metrics, counts, recent posts, and notifications.</returns>
    [HttpGet("{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboardSummary(int workspaceId)
    {
        var summary = await _dashboardService.GetDashboardSummaryAsync(workspaceId);
        return Ok(ApiResponse<DashboardSummaryResponse>.Ok(summary, "Dashboard summary retrieved successfully."));
    }
}
