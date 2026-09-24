namespace SocialHub.Api.Models;

public class AnalyticsSnapshot
{
    public int Id { get; set; }
    public int SocialAccountId { get; set; }
    public DateTime Date { get; set; }
    public int Followers { get; set; }
    public int Reach { get; set; }
    public int Impressions { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Views { get; set; }
    public double EngagementRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public SocialAccount SocialAccount { get; set; } = null!;
}
