using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs;

public class CreateSocialAccountRequest
{
    [Required(ErrorMessage = "Workspace ID is required.")]
    public int WorkspaceId { get; set; }

    [Required(ErrorMessage = "Platform is required.")]
    [RegularExpression(@"^(Facebook|Instagram|TikTok|YouTube)$", ErrorMessage = "Platform must be one of: Facebook, Instagram, TikTok, YouTube.")]
    public string Platform { get; set; } = string.Empty;

    [Required(ErrorMessage = "Platform account ID is required.")]
    [StringLength(100, ErrorMessage = "Platform account ID cannot exceed 100 characters.")]
    public string PlatformAccountId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(150, ErrorMessage = "Display name cannot exceed 150 characters.")]
    public string DisplayName { get; set; } = string.Empty;

    [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Avatar URL cannot exceed 1000 characters.")]
    public string? AvatarUrl { get; set; }
}

public class UpdateSocialAccountRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(150, ErrorMessage = "Display name cannot exceed 150 characters.")]
    public string DisplayName { get; set; } = string.Empty;

    [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Avatar URL cannot exceed 1000 characters.")]
    public string? AvatarUrl { get; set; }
}

public class SocialAccountResponse
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string PlatformAccountId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsConnected { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime? ConnectedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
