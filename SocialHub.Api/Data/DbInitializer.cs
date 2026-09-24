using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Models;

namespace SocialHub.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(SocialHubDbContext context)
    {
        // Check if database is already seeded
        if (await context.Workspaces.AnyAsync())
        {
            return;
        }

        var baseTime = DateTime.UtcNow;

        // 1. Workspace
        var workspace = new Workspace
        {
            Name = "DevMind Marketing",
            Slug = "devmind-marketing",
            Description = "Full-service digital marketing and social media growth agency.",
            LogoUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=150",
            IsActive = true,
            CreatedAt = baseTime.AddDays(-35)
        };
        context.Workspaces.Add(workspace);
        await context.SaveChangesAsync();

        // 2. User
        var user = new User
        {
            WorkspaceId = workspace.Id,
            FullName = "Alex Mercer",
            Email = "alex.mercer@devmind.com",
            AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150",
            IsActive = true,
            CreatedAt = baseTime.AddDays(-35)
        };
        context.Users.Add(user);

        // 3. Social Accounts
        var fbAccount = new SocialAccount
        {
            WorkspaceId = workspace.Id,
            Platform = SocialPlatforms.Facebook,
            PlatformAccountId = "fb_1092837465",
            Username = "devmind.agency",
            DisplayName = "DevMind Marketing Agency",
            AvatarUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=150",
            IsConnected = true,
            ConnectedAt = baseTime.AddDays(-30),
            AccessToken = $"mock_fb_access_token_{Guid.NewGuid():N}",
            RefreshToken = $"mock_fb_refresh_token_{Guid.NewGuid():N}",
            TokenExpiresAt = baseTime.AddDays(60),
            CreatedAt = baseTime.AddDays(-30)
        };

        var igAccount = new SocialAccount
        {
            WorkspaceId = workspace.Id,
            Platform = SocialPlatforms.Instagram,
            PlatformAccountId = "ig_9876543210",
            Username = "devmind_marketing",
            DisplayName = "DevMind Marketing Official",
            AvatarUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=150",
            IsConnected = true,
            ConnectedAt = baseTime.AddDays(-30),
            AccessToken = $"mock_ig_access_token_{Guid.NewGuid():N}",
            RefreshToken = $"mock_ig_refresh_token_{Guid.NewGuid():N}",
            TokenExpiresAt = baseTime.AddDays(60),
            CreatedAt = baseTime.AddDays(-30)
        };

        var ttAccount = new SocialAccount
        {
            WorkspaceId = workspace.Id,
            Platform = SocialPlatforms.TikTok,
            PlatformAccountId = "tt_5432167890",
            Username = "devmind_creative",
            DisplayName = "DevMind Creative Studio",
            AvatarUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=150",
            IsConnected = true,
            ConnectedAt = baseTime.AddDays(-30),
            AccessToken = $"mock_tt_access_token_{Guid.NewGuid():N}",
            RefreshToken = $"mock_tt_refresh_token_{Guid.NewGuid():N}",
            TokenExpiresAt = baseTime.AddDays(60),
            CreatedAt = baseTime.AddDays(-30)
        };

        var ytAccount = new SocialAccount
        {
            WorkspaceId = workspace.Id,
            Platform = SocialPlatforms.YouTube,
            PlatformAccountId = "yt_1357924680",
            Username = "devmind_media",
            DisplayName = "DevMind Media Channel",
            AvatarUrl = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=150",
            IsConnected = true,
            ConnectedAt = baseTime.AddDays(-30),
            AccessToken = $"mock_yt_access_token_{Guid.NewGuid():N}",
            RefreshToken = $"mock_yt_refresh_token_{Guid.NewGuid():N}",
            TokenExpiresAt = baseTime.AddDays(60),
            CreatedAt = baseTime.AddDays(-30)
        };

        context.SocialAccounts.AddRange(fbAccount, igAccount, ttAccount, ytAccount);
        await context.SaveChangesAsync();

        // 4. Media
        var mediaImage = new Media
        {
            WorkspaceId = workspace.Id,
            FileName = "summer-campaign-banner.jpg",
            FileUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800",
            MimeType = "image/jpeg",
            FileSize = 2450123,
            Duration = null,
            CreatedAt = baseTime.AddDays(-10)
        };

        var mediaVideo = new Media
        {
            WorkspaceId = workspace.Id,
            FileName = "product-launch-teaser.mp4",
            FileUrl = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4",
            MimeType = "video/mp4",
            FileSize = 15890200,
            Duration = 30,
            CreatedAt = baseTime.AddDays(-5)
        };

        context.MediaItems.AddRange(mediaImage, mediaVideo);
        await context.SaveChangesAsync();

        // 5. Hashtags
        var tagSocial = new Hashtag { Name = "#socialmedia", CreatedAt = baseTime.AddDays(-30) };
        var tagMarketing = new Hashtag { Name = "#digitalmarketing", CreatedAt = baseTime.AddDays(-30) };
        var tagGrowth = new Hashtag { Name = "#growth", CreatedAt = baseTime.AddDays(-30) };
        var tagBranding = new Hashtag { Name = "#branding", CreatedAt = baseTime.AddDays(-30) };
        var tagDevMind = new Hashtag { Name = "#devmind", CreatedAt = baseTime.AddDays(-30) };

        context.Hashtags.AddRange(tagSocial, tagMarketing, tagGrowth, tagBranding, tagDevMind);
        await context.SaveChangesAsync();

        // 6. Posts
        // Post 1: Published
        var post1 = new Post
        {
            WorkspaceId = workspace.Id,
            Title = "Announcing Our 2026 Digital Growth Strategy",
            Caption = "We are thrilled to unveil our comprehensive 2026 digital marketing playbook designed to elevate your brand presence across all modern channels. Check out the link in bio! #socialmedia #digitalmarketing #growth",
            Status = PostStatuses.Published,
            MediaId = mediaImage.Id,
            CreatedAt = baseTime.AddDays(-2),
            PublishedAt = baseTime.AddDays(-2)
        };
        post1.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = fbAccount.Id,
            Platform = SocialPlatforms.Facebook,
            Status = PlatformPostStatuses.Published,
            ExternalPostId = "mock_fb_post_90218",
            PublishedAt = baseTime.AddDays(-2),
            CreatedAt = baseTime.AddDays(-2)
        });
        post1.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = igAccount.Id,
            Platform = SocialPlatforms.Instagram,
            Status = PlatformPostStatuses.Published,
            ExternalPostId = "mock_ig_post_83019",
            PublishedAt = baseTime.AddDays(-2),
            CreatedAt = baseTime.AddDays(-2)
        });
        post1.PostHashtags.Add(new PostHashtag { HashtagId = tagSocial.Id });
        post1.PostHashtags.Add(new PostHashtag { HashtagId = tagMarketing.Id });
        post1.PostHashtags.Add(new PostHashtag { HashtagId = tagGrowth.Id });

        // Post 2: Scheduled
        var post2 = new Post
        {
            WorkspaceId = workspace.Id,
            Title = "Behind the Scenes at DevMind Studios",
            Caption = "Take a sneak peek into how our content creators shoot and edit high-impact short-form videos for multi-platform distribution. #branding #devmind",
            Status = PostStatuses.Scheduled,
            MediaId = mediaVideo.Id,
            CreatedAt = baseTime.AddDays(-1),
            ScheduledAt = baseTime.AddDays(3)
        };
        post2.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = ttAccount.Id,
            Platform = SocialPlatforms.TikTok,
            Status = PlatformPostStatuses.Pending,
            CreatedAt = baseTime.AddDays(-1)
        });
        post2.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = ytAccount.Id,
            Platform = SocialPlatforms.YouTube,
            Status = PlatformPostStatuses.Pending,
            CreatedAt = baseTime.AddDays(-1)
        });
        post2.PostHashtags.Add(new PostHashtag { HashtagId = tagBranding.Id });
        post2.PostHashtags.Add(new PostHashtag { HashtagId = tagDevMind.Id });

        // Post 3: Draft
        var post3 = new Post
        {
            WorkspaceId = workspace.Id,
            Title = "Q4 Social Commerce Trends You Need to Know",
            Caption = "Drafting our insights on social commerce integration, livestream shopping, and algorithmic trends heading into the holiday shopping season.",
            Status = PostStatuses.Draft,
            MediaId = null,
            CreatedAt = baseTime.AddHours(-12)
        };
        post3.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = fbAccount.Id,
            Platform = SocialPlatforms.Facebook,
            Status = PlatformPostStatuses.Pending,
            CreatedAt = baseTime.AddHours(-12)
        });
        post3.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = igAccount.Id,
            Platform = SocialPlatforms.Instagram,
            Status = PlatformPostStatuses.Pending,
            CreatedAt = baseTime.AddHours(-12)
        });

        // Post 4: Failed
        var post4 = new Post
        {
            WorkspaceId = workspace.Id,
            Title = "Flash Sale Announcement - 48 Hours Only",
            Caption = "Limited time discount on our full-funnel marketing audit package. Redeem now! #growth",
            Status = PostStatuses.Failed,
            MediaId = mediaImage.Id,
            CreatedAt = baseTime.AddHours(-6)
        };
        post4.PostPlatforms.Add(new PostPlatform
        {
            SocialAccountId = ttAccount.Id,
            Platform = SocialPlatforms.TikTok,
            Status = PlatformPostStatuses.Failed,
            ErrorMessage = "Simulated API error: TikTok video duration requirement not met (code 400).",
            CreatedAt = baseTime.AddHours(-6)
        });
        post4.PostHashtags.Add(new PostHashtag { HashtagId = tagGrowth.Id });

        context.Posts.AddRange(post1, post2, post3, post4);
        await context.SaveChangesAsync();

        // 7. Notifications
        var notifications = new List<Notification>
        {
            new Notification
            {
                WorkspaceId = workspace.Id,
                Title = "Post Published Successfully",
                Message = "Post 'Announcing Our 2026 Digital Growth Strategy' was published to Facebook and Instagram.",
                Type = NotificationTypes.Success,
                IsRead = true,
                CreatedAt = baseTime.AddDays(-2)
            },
            new Notification
            {
                WorkspaceId = workspace.Id,
                Title = "Post Scheduled",
                Message = "Post 'Behind the Scenes at DevMind Studios' is scheduled for publishing on TikTok and YouTube.",
                Type = NotificationTypes.Info,
                IsRead = false,
                CreatedAt = baseTime.AddDays(-1)
            },
            new Notification
            {
                WorkspaceId = workspace.Id,
                Title = "Publishing Failed",
                Message = "Post 'Flash Sale Announcement' failed to publish to TikTok due to video duration requirements.",
                Type = NotificationTypes.Error,
                IsRead = false,
                CreatedAt = baseTime.AddHours(-6)
            },
            new Notification
            {
                WorkspaceId = workspace.Id,
                Title = "Token Expiry Reminder",
                Message = "Facebook account token will require re-authorization in 30 days.",
                Type = NotificationTypes.Warning,
                IsRead = false,
                CreatedAt = baseTime.AddHours(-2)
            }
        };

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();

        // 8. Deterministic Analytics Snapshots for the last 30 days
        var snapshots = new List<AnalyticsSnapshot>();

        for (int day = 30; day >= 1; day--)
        {
            var date = baseTime.AddDays(-day).Date;
            int step = 31 - day; // 1 to 30

            // Facebook
            snapshots.Add(new AnalyticsSnapshot
            {
                SocialAccountId = fbAccount.Id,
                Date = date,
                Followers = 12000 + (step * 18),
                Reach = 1100 + (step * 15) + (step % 5 * 60),
                Impressions = 2200 + (step * 30) + (step % 4 * 120),
                Likes = 75 + (step * 2) + (step % 3 * 5),
                Comments = 10 + (step % 4 * 3),
                Shares = 5 + (step % 3 * 2),
                Views = 850 + (step * 10),
                EngagementRate = 4.2 + (step % 3 * 0.1),
                CreatedAt = date.AddHours(23)
            });

            // Instagram
            snapshots.Add(new AnalyticsSnapshot
            {
                SocialAccountId = igAccount.Id,
                Date = date,
                Followers = 27000 + (step * 45),
                Reach = 3200 + (step * 40) + (step % 5 * 150),
                Impressions = 6800 + (step * 75) + (step % 4 * 250),
                Likes = 280 + (step * 6) + (step % 3 * 15),
                Comments = 38 + (step % 4 * 5),
                Shares = 22 + (step % 3 * 4),
                Views = 3800 + (step * 35),
                EngagementRate = 5.4 + (step % 3 * 0.1),
                CreatedAt = date.AddHours(23)
            });

            // TikTok
            snapshots.Add(new AnalyticsSnapshot
            {
                SocialAccountId = ttAccount.Id,
                Date = date,
                Followers = 42000 + (step * 95),
                Reach = 11000 + (step * 120) + (step % 5 * 400),
                Impressions = 23000 + (step * 250) + (step % 4 * 800),
                Likes = 1100 + (step * 15) + (step % 3 * 40),
                Comments = 120 + (step % 4 * 12),
                Shares = 280 + (step % 3 * 25),
                Views = 20000 + (step * 220),
                EngagementRate = 6.7 + (step % 3 * 0.15),
                CreatedAt = date.AddHours(23)
            });

            // YouTube
            snapshots.Add(new AnalyticsSnapshot
            {
                SocialAccountId = ytAccount.Id,
                Date = date,
                Followers = 8000 + (step * 9),
                Reach = 750 + (step * 8) + (step % 5 * 30),
                Impressions = 1650 + (step * 20) + (step % 4 * 70),
                Likes = 85 + (step * 2) + (step % 3 * 6),
                Comments = 15 + (step % 4 * 2),
                Shares = 12 + (step % 3 * 2),
                Views = 1400 + (step * 15),
                EngagementRate = 3.7 + (step % 3 * 0.1),
                CreatedAt = date.AddHours(23)
            });
        }

        context.AnalyticsSnapshots.AddRange(snapshots);
        await context.SaveChangesAsync();
    }
}
