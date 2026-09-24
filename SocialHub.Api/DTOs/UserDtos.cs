using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Avatar URL cannot exceed 1000 characters.")]
    public string? AvatarUrl { get; set; }

    public int? WorkspaceId { get; set; }
}

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Avatar URL cannot exceed 1000 characters.")]
    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int? WorkspaceId { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public int? WorkspaceId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
