namespace SocialHub.Api.Models;

public static class SocialPlatforms
{
    public const string Facebook = "Facebook";
    public const string Instagram = "Instagram";
    public const string TikTok = "TikTok";
    public const string YouTube = "YouTube";

    public static readonly string[] All = { Facebook, Instagram, TikTok, YouTube };

    public static bool IsValid(string platform) =>
        All.Contains(platform, StringComparer.OrdinalIgnoreCase);
}

public static class PostStatuses
{
    public const string Draft = "Draft";
    public const string Processing = "Processing";
    public const string Published = "Published";
    public const string Scheduled = "Scheduled";
    public const string Failed = "Failed";

    public static readonly string[] All = { Draft, Processing, Published, Scheduled, Failed };

    public static bool IsValid(string status) =>
        All.Contains(status, StringComparer.OrdinalIgnoreCase);
}

public static class PlatformPostStatuses
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Published = "Published";
    public const string Failed = "Failed";

    public static readonly string[] All = { Pending, Processing, Published, Failed };

    public static bool IsValid(string status) =>
        All.Contains(status, StringComparer.OrdinalIgnoreCase);
}

public static class NotificationTypes
{
    public const string Info = "Info";
    public const string Success = "Success";
    public const string Warning = "Warning";
    public const string Error = "Error";

    public static readonly string[] All = { Info, Success, Warning, Error };

    public static bool IsValid(string type) =>
        All.Contains(type, StringComparer.OrdinalIgnoreCase);
}
