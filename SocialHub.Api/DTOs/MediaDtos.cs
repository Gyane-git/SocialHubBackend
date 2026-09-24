using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs;

public class CreateMediaRequest
{
    [Required(ErrorMessage = "Workspace ID is required.")]
    public int WorkspaceId { get; set; }

    [Required(ErrorMessage = "File name is required.")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "File URL is required.")]
    [Url(ErrorMessage = "File URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "File URL cannot exceed 1000 characters.")]
    public string FileUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "MIME type is required.")]
    [StringLength(100, ErrorMessage = "MIME type cannot exceed 100 characters.")]
    public string MimeType { get; set; } = string.Empty;

    [Required(ErrorMessage = "File size is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0 bytes.")]
    public long FileSize { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Duration must be positive.")]
    public int? Duration { get; set; }
}

public class UpdateMediaRequest
{
    [Required(ErrorMessage = "File name is required.")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "File URL is required.")]
    [Url(ErrorMessage = "File URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "File URL cannot exceed 1000 characters.")]
    public string FileUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "MIME type is required.")]
    [StringLength(100, ErrorMessage = "MIME type cannot exceed 100 characters.")]
    public string MimeType { get; set; } = string.Empty;

    [Required(ErrorMessage = "File size is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0 bytes.")]
    public long FileSize { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Duration must be positive.")]
    public int? Duration { get; set; }
}

public class MediaResponse
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? Duration { get; set; }
    public DateTime CreatedAt { get; set; }
}
