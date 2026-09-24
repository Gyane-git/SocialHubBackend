namespace SocialHub.Api.Models;

public class PostHashtag
{
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int HashtagId { get; set; }
    public Hashtag Hashtag { get; set; } = null!;
}
