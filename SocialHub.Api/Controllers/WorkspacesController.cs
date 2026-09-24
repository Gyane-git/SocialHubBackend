using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspacesController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    /// <summary>
    /// Get all workspaces.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkspaceResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var workspaces = await _workspaceService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<WorkspaceResponse>>.Ok(workspaces, "Workspaces retrieved successfully."));
    }

    /// <summary>
    /// Get workspace by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkspaceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var workspace = await _workspaceService.GetByIdAsync(id);
        return Ok(ApiResponse<WorkspaceResponse>.Ok(workspace, "Workspace retrieved successfully."));
    }

    /// <summary>
    /// Create a new workspace.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorkspaceResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequest request)
    {
        var created = await _workspaceService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<WorkspaceResponse>.Ok(created, "Workspace created successfully."));
    }

    /// <summary>
    /// Update an existing workspace.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkspaceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkspaceRequest request)
    {
        var updated = await _workspaceService.UpdateAsync(id, request);
        return Ok(ApiResponse<WorkspaceResponse>.Ok(updated, "Workspace updated successfully."));
    }

    /// <summary>
    /// Delete a workspace by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _workspaceService.DeleteAsync(id);
        return NoContent();
    }
}
