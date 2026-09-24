namespace SocialHub.Api.Models;

public class SocialAccount
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string PlatformAccountId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsConnected { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public ICollection<PostPlatform> PostPlatforms { get; set; } = new List<PostPlatform>();
    public ICollection<AnalyticsSnapshot> AnalyticsSnapshots { get; set; } = new List<AnalyticsSnapshot>();
}
