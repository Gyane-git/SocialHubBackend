using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs;

public class CreateWorkspaceRequest
{
    [Required(ErrorMessage = "Workspace name is required.")]
    [StringLength(150, ErrorMessage = "Workspace name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Workspace slug is required.")]
    [StringLength(100, ErrorMessage = "Workspace slug cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, numbers, and single hyphens.")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Logo URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Logo URL cannot exceed 1000 characters.")]
    public string? LogoUrl { get; set; }
}

public class UpdateWorkspaceRequest
{
    [Required(ErrorMessage = "Workspace name is required.")]
    [StringLength(150, ErrorMessage = "Workspace name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Workspace slug is required.")]
    [StringLength(100, ErrorMessage = "Workspace slug cannot exceed 100 characters.")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must contain only lowercase letters, numbers, and single hyphens.")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Logo URL must be a valid URL.")]
    [StringLength(1000, ErrorMessage = "Logo URL cannot exceed 1000 characters.")]
    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; } = true;
}

public class WorkspaceResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
