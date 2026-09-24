namespace SocialHub.Api.Models;

public class Media
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? Duration { get; set; } // Duration in seconds (for video/audio)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Workspace Workspace { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
