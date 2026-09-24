using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HashtagsController : ControllerBase
{
    private readonly IHashtagService _hashtagService;

    public HashtagsController(IHashtagService hashtagService)
    {
        _hashtagService = hashtagService;
    }

    /// <summary>
    /// Get all hashtags.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HashtagResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var hashtags = await _hashtagService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<HashtagResponse>>.Ok(hashtags, "Hashtags retrieved successfully."));
    }

    /// <summary>
    /// Create a new hashtag.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HashtagResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHashtagRequest request)
    {
        var created = await _hashtagService.CreateAsync(request);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<HashtagResponse>.Ok(created, "Hashtag created successfully."));
    }

    /// <summary>
    /// Delete a hashtag by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _hashtagService.DeleteAsync(id);
        return NoContent();
    }
}
