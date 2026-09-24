using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    /// <summary>
    /// Get all media items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MediaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var media = await _mediaService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<MediaResponse>>.Ok(media, "Media items retrieved successfully."));
    }

    /// <summary>
    /// Get media item by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MediaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var media = await _mediaService.GetByIdAsync(id);
        return Ok(ApiResponse<MediaResponse>.Ok(media, "Media item retrieved successfully."));
    }

    /// <summary>
    /// Get all media items for a specific workspace.
    /// </summary>
    [HttpGet("workspace/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MediaResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var media = await _mediaService.GetByWorkspaceIdAsync(workspaceId);
        return Ok(ApiResponse<IEnumerable<MediaResponse>>.Ok(media, "Workspace media items retrieved successfully."));
    }

    /// <summary>
    /// Create media item metadata.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MediaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateMediaRequest request)
    {
        var created = await _mediaService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<MediaResponse>.Ok(created, "Media item created successfully."));
    }

    /// <summary>
    /// Update existing media item metadata.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MediaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMediaRequest request)
    {
        var updated = await _mediaService.UpdateAsync(id, request);
        return Ok(ApiResponse<MediaResponse>.Ok(updated, "Media item updated successfully."));
    }

    /// <summary>
    /// Delete a media item by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediaService.DeleteAsync(id);
        return NoContent();
    }
}
