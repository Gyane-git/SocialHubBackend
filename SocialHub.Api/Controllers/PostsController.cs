using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Get all posts with optional filtering by workspace, status, platform, and text search.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PostQueryParameters parameters)
    {
        var posts = await _postService.GetAllAsync(parameters);
        return Ok(ApiResponse<IEnumerable<PostResponse>>.Ok(posts, "Posts retrieved successfully."));
    }

    /// <summary>
    /// Get post by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _postService.GetByIdAsync(id);
        return Ok(ApiResponse<PostResponse>.Ok(post, "Post retrieved successfully."));
    }

    /// <summary>
    /// Get all posts for a specific workspace.
    /// </summary>
    [HttpGet("workspace/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PostResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var posts = await _postService.GetByWorkspaceIdAsync(workspaceId);
        return Ok(ApiResponse<IEnumerable<PostResponse>>.Ok(posts, "Workspace posts retrieved successfully."));
    }

    /// <summary>
    /// Create a new post.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var created = await _postService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<PostResponse>.Ok(created, "Post created successfully."));
    }

    /// <summary>
    /// Update an existing post.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
    {
        var updated = await _postService.UpdateAsync(id, request);
        return Ok(ApiResponse<PostResponse>.Ok(updated, "Post updated successfully."));
    }

    /// <summary>
    /// Delete a post by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _postService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Publish a post (Mock operation).
    /// </summary>
    [HttpPost("{id:int}/publish")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(int id)
    {
        var published = await _postService.PublishAsync(id);
        return Ok(ApiResponse<PostResponse>.Ok(published, "Post published successfully."));
    }

    /// <summary>
    /// Schedule a post for future publishing.
    /// </summary>
    [HttpPost("{id:int}/schedule")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Schedule(int id, [FromBody] SchedulePostRequest request)
    {
        var scheduled = await _postService.ScheduleAsync(id, request);
        return Ok(ApiResponse<PostResponse>.Ok(scheduled, "Post scheduled successfully."));
    }

    /// <summary>
    /// Duplicate a post as a new draft.
    /// </summary>
    [HttpPost("{id:int}/duplicate")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Duplicate(int id)
    {
        var duplicated = await _postService.DuplicateAsync(id);
        return CreatedAtAction(nameof(GetById), new { id = duplicated.Id }, ApiResponse<PostResponse>.Ok(duplicated, "Post duplicated successfully as draft."));
    }

    /// <summary>
    /// Retry publishing a failed post.
    /// </summary>
    [HttpPost("{id:int}/retry")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retry(int id)
    {
        var retried = await _postService.RetryAsync(id);
        return Ok(ApiResponse<PostResponse>.Ok(retried, "Post publishing retry initiated successfully."));
    }
}
