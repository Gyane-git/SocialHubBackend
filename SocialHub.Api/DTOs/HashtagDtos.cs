using System.ComponentModel.DataAnnotations;

namespace SocialHub.Api.DTOs;

public class CreateHashtagRequest
{
    [Required(ErrorMessage = "Hashtag name is required.")]
    [StringLength(100, ErrorMessage = "Hashtag name cannot exceed 100 characters.")]
    [RegularExpression(@"^#?[a-zA-Z0-9_]+$", ErrorMessage = "Hashtag must contain only letters, numbers, and underscores, optionally starting with #.")]
    public string Name { get; set; } = string.Empty;
}

public class HashtagResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
