using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/social-accounts")]
[Produces("application/json")]
public class SocialAccountsController : ControllerBase
{
    private readonly ISocialAccountService _socialAccountService;

    public SocialAccountsController(ISocialAccountService socialAccountService)
    {
        _socialAccountService = socialAccountService;
    }

    /// <summary>
    /// Get all social accounts.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SocialAccountResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await _socialAccountService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SocialAccountResponse>>.Ok(accounts, "Social accounts retrieved successfully."));
    }

    /// <summary>
    /// Get social account by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SocialAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _socialAccountService.GetByIdAsync(id);
        return Ok(ApiResponse<SocialAccountResponse>.Ok(account, "Social account retrieved successfully."));
    }

    /// <summary>
    /// Get all social accounts for a specific workspace.
    /// </summary>
    [HttpGet("workspace/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SocialAccountResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var accounts = await _socialAccountService.GetByWorkspaceIdAsync(workspaceId);
        return Ok(ApiResponse<IEnumerable<SocialAccountResponse>>.Ok(accounts, "Workspace social accounts retrieved successfully."));
    }

    /// <summary>
    /// Create a new social account.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SocialAccountResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSocialAccountRequest request)
    {
        var created = await _socialAccountService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<SocialAccountResponse>.Ok(created, "Social account created successfully."));
    }

    /// <summary>
    /// Update an existing social account.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SocialAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSocialAccountRequest request)
    {
        var updated = await _socialAccountService.UpdateAsync(id, request);
        return Ok(ApiResponse<SocialAccountResponse>.Ok(updated, "Social account updated successfully."));
    }

    /// <summary>
    /// Delete a social account by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _socialAccountService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Connect a social account (Mock operation).
    /// </summary>
    [HttpPost("{id:int}/connect")]
    [ProducesResponseType(typeof(ApiResponse<SocialAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Connect(int id)
    {
        var account = await _socialAccountService.ConnectAsync(id);
        return Ok(ApiResponse<SocialAccountResponse>.Ok(account, "Social account connected successfully."));
    }

    /// <summary>
    /// Disconnect a social account (Mock operation).
    /// </summary>
    [HttpPost("{id:int}/disconnect")]
    [ProducesResponseType(typeof(ApiResponse<SocialAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Disconnect(int id)
    {
        var account = await _socialAccountService.DisconnectAsync(id);
        return Ok(ApiResponse<SocialAccountResponse>.Ok(account, "Social account disconnected successfully."));
    }
}
