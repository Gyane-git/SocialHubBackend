namespace SocialHub.Api.Models;

public class Post
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Status { get; set; } = PostStatuses.Draft;
    public int? MediaId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public Media? Media { get; set; }
    public ICollection<PostPlatform> PostPlatforms { get; set; } = new List<PostPlatform>();
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}
