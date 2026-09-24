using System.Text.Json.Serialization;

namespace SocialHub.Api.DTOs;

public class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "ok";

    [JsonPropertyName("service")]
    public string Service { get; set; } = "SocialHub API";

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
}
