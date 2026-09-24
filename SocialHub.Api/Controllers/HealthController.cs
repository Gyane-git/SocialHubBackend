using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.DTOs;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint to verify API readiness.
    /// </summary>
    /// <returns>Health status, service name, and UTC timestamp.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResponse
        {
            Status = "ok",
            Service = "SocialHub API",
            Timestamp = DateTime.UtcNow.ToString("o")
        });
    }
}
