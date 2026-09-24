namespace SocialHub.Api.Models;

public class PostPlatform
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int SocialAccountId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Status { get; set; } = PlatformPostStatuses.Pending;
    public string? ExternalPostId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Post Post { get; set; } = null!;
    public SocialAccount SocialAccount { get; set; } = null!;
}
