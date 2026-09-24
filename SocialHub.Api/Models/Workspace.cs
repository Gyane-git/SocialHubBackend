namespace SocialHub.Api.Models;

public class Workspace
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<SocialAccount> SocialAccounts { get; set; } = new List<SocialAccount>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Media> MediaItems { get; set; } = new List<Media>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
