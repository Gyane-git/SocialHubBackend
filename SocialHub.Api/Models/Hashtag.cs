namespace SocialHub.Api.Models;

public class Hashtag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}
