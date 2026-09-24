namespace SocialHub.Api.DTOs;

public class DashboardSummaryResponse
{
    public int WorkspaceId { get; set; }
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int ScheduledPosts { get; set; }
    public int FailedPosts { get; set; }
    public int ConnectedAccounts { get; set; }
    public long TotalReach { get; set; }
    public long TotalImpressions { get; set; }
    public long TotalEngagement { get; set; }
    public long TotalViews { get; set; }
    public List<PostResponse> RecentPosts { get; set; } = new();
    public List<NotificationResponse> RecentNotifications { get; set; } = new();
}
