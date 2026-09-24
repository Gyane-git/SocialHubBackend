using System.ComponentModel.DataAnnotations;
using SocialHub.Api.Models;

namespace SocialHub.Api.DTOs;

public class CreatePostRequest
{
    [Required(ErrorMessage = "Workspace ID is required.")]
    public int WorkspaceId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Caption is required.")]
    [StringLength(4000, ErrorMessage = "Caption cannot exceed 4000 characters.")]
    public string Caption { get; set; } = string.Empty;

    public string? Status { get; set; } = PostStatuses.Draft;

    public int? MediaId { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public List<int> SocialAccountIds { get; set; } = new();

    public List<string> Hashtags { get; set; } = new();
}

public class UpdatePostRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(250, ErrorMessage = "Title cannot exceed 250 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Caption is required.")]
    [StringLength(4000, ErrorMessage = "Caption cannot exceed 4000 characters.")]
    public string Caption { get; set; } = string.Empty;

    public string? Status { get; set; }

    public int? MediaId { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public List<int>? SocialAccountIds { get; set; }

    public List<string>? Hashtags { get; set; }
}

public class SchedulePostRequest
{
    [Required(ErrorMessage = "ScheduledAt date and time is required.")]
    public DateTime ScheduledAt { get; set; }
}

public class PostPlatformResponse
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int SocialAccountId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalPostId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PostResponse
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? MediaId { get; set; }
    public MediaResponse? Media { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PostPlatformResponse> Platforms { get; set; } = new();
    public List<string> Hashtags { get; set; } = new();
}

public class PostQueryParameters
{
    public int? WorkspaceId { get; set; }
    public string? Status { get; set; }
    public string? Platform { get; set; }
    public string? Search { get; set; }
}
