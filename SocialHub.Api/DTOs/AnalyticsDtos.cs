namespace SocialHub.Api.DTOs;

public class AnalyticsSnapshotResponse
{
    public int Id { get; set; }
    public int SocialAccountId { get; set; }
    public string? Platform { get; set; }
    public string? Username { get; set; }
    public DateTime Date { get; set; }
    public int Followers { get; set; }
    public int Reach { get; set; }
    public int Impressions { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Views { get; set; }
    public double EngagementRate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PlatformMetricsResponse
{
    public string Platform { get; set; } = string.Empty;
    public int Followers { get; set; }
    public long Reach { get; set; }
    public long Impressions { get; set; }
    public long Likes { get; set; }
    public long Comments { get; set; }
    public long Shares { get; set; }
    public long Engagement { get; set; }
    public long Views { get; set; }
    public double AverageEngagementRate { get; set; }
}

public class AnalyticsSummaryResponse
{
    public int WorkspaceId { get; set; }
    public int TotalFollowers { get; set; }
    public long TotalReach { get; set; }
    public long TotalImpressions { get; set; }
    public long TotalLikes { get; set; }
    public long TotalComments { get; set; }
    public long TotalShares { get; set; }
    public long TotalEngagement { get; set; }
    public long TotalViews { get; set; }
    public double AverageEngagementRate { get; set; }
    public List<PlatformMetricsResponse> ByPlatform { get; set; } = new();
}
