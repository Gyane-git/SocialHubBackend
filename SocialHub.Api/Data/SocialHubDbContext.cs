using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Models;

namespace SocialHub.Api.Data;

public class SocialHubDbContext : DbContext
{
    public SocialHubDbContext(DbContextOptions<SocialHubDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<SocialAccount> SocialAccounts => Set<SocialAccount>();
    public DbSet<Media> MediaItems => Set<Media>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostPlatform> PostPlatforms => Set<PostPlatform>();
    public DbSet<Hashtag> Hashtags => Set<Hashtag>();
    public DbSet<PostHashtag> PostHashtags => Set<PostHashtag>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AnalyticsSnapshot> AnalyticsSnapshots => Set<AnalyticsSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).HasMaxLength(500);
            entity.Property(u => u.AvatarUrl).HasMaxLength(1000);

            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Workspace)
                .WithMany(w => w.Users)
                .HasForeignKey(u => u.WorkspaceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Workspace
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Name).IsRequired().HasMaxLength(150);
            entity.Property(w => w.Slug).IsRequired().HasMaxLength(100);
            entity.Property(w => w.Description).HasMaxLength(500);
            entity.Property(w => w.LogoUrl).HasMaxLength(1000);

            entity.HasIndex(w => w.Slug).IsUnique();
        });

        // SocialAccount
        modelBuilder.Entity<SocialAccount>(entity =>
        {
            entity.HasKey(sa => sa.Id);
            entity.Property(sa => sa.Platform).IsRequired().HasMaxLength(50);
            entity.Property(sa => sa.PlatformAccountId).IsRequired().HasMaxLength(100);
            entity.Property(sa => sa.Username).IsRequired().HasMaxLength(100);
            entity.Property(sa => sa.DisplayName).IsRequired().HasMaxLength(150);
            entity.Property(sa => sa.AvatarUrl).HasMaxLength(1000);
            entity.Property(sa => sa.AccessToken).HasMaxLength(2000);
            entity.Property(sa => sa.RefreshToken).HasMaxLength(2000);

            entity.HasIndex(sa => new { sa.WorkspaceId, sa.Platform, sa.PlatformAccountId }).IsUnique();

            entity.HasOne(sa => sa.Workspace)
                .WithMany(w => w.SocialAccounts)
                .HasForeignKey(sa => sa.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Media
        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.FileName).IsRequired().HasMaxLength(255);
            entity.Property(m => m.FileUrl).IsRequired().HasMaxLength(1000);
            entity.Property(m => m.MimeType).IsRequired().HasMaxLength(100);

            entity.HasOne(m => m.Workspace)
                .WithMany(w => w.MediaItems)
                .HasForeignKey(m => m.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Post
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Caption).HasMaxLength(4000);
            entity.Property(p => p.Status).IsRequired().HasMaxLength(50);

            entity.HasIndex(p => new { p.WorkspaceId, p.Status });

            entity.HasOne(p => p.Workspace)
                .WithMany(w => w.Posts)
                .HasForeignKey(p => p.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Media)
                .WithMany(m => m.Posts)
                .HasForeignKey(p => p.MediaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PostPlatform
        modelBuilder.Entity<PostPlatform>(entity =>
        {
            entity.HasKey(pp => pp.Id);
            entity.Property(pp => pp.Platform).IsRequired().HasMaxLength(50);
            entity.Property(pp => pp.Status).IsRequired().HasMaxLength(50);
            entity.Property(pp => pp.ExternalPostId).HasMaxLength(100);
            entity.Property(pp => pp.ErrorMessage).HasMaxLength(1000);

            entity.HasOne(pp => pp.Post)
                .WithMany(p => p.PostPlatforms)
                .HasForeignKey(pp => pp.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pp => pp.SocialAccount)
                .WithMany(sa => sa.PostPlatforms)
                .HasForeignKey(pp => pp.SocialAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Hashtag
        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(h => h.Name).IsUnique();
        });

        // PostHashtag
        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.HasKey(ph => new { ph.PostId, ph.HashtagId });

            entity.HasOne(ph => ph.Post)
                .WithMany(p => p.PostHashtags)
                .HasForeignKey(ph => ph.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ph => ph.Hashtag)
                .WithMany(h => h.PostHashtags)
                .HasForeignKey(ph => ph.HashtagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            entity.Property(n => n.Type).IsRequired().HasMaxLength(50);

            entity.HasOne(n => n.Workspace)
                .WithMany(w => w.Notifications)
                .HasForeignKey(n => n.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AnalyticsSnapshot
        modelBuilder.Entity<AnalyticsSnapshot>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.EngagementRate).HasPrecision(5, 2);

            entity.HasIndex(a => new { a.SocialAccountId, a.Date });

            entity.HasOne(a => a.SocialAccount)
                .WithMany(sa => sa.AnalyticsSnapshots)
                .HasForeignKey(a => a.SocialAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
