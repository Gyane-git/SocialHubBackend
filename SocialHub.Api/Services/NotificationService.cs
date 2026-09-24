using Microsoft.EntityFrameworkCore;
using SocialHub.Api.Common;
using SocialHub.Api.Data;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;
using SocialHub.Api.Models;

namespace SocialHub.Api.Services;

public class NotificationService : INotificationService
{
    private readonly SocialHubDbContext _context;

    public NotificationService(SocialHubDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationResponse>> GetAllAsync()
    {
        return await _context.Notifications
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToResponse(n))
            .ToListAsync();
    }

    public async Task<IEnumerable<NotificationResponse>> GetByWorkspaceIdAsync(int workspaceId)
    {
        var workspaceExists = await _context.Workspaces.AnyAsync(w => w.Id == workspaceId);
        if (!workspaceExists)
        {
            throw new NotFoundException($"Workspace with ID {workspaceId} was not found.");
        }

        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.WorkspaceId == workspaceId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToResponse(n))
            .ToListAsync();
    }

    public async Task<NotificationResponse> MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            throw new NotFoundException($"Notification with ID {id} was not found.");
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return MapToResponse(notification);
    }

    public async Task<int> MarkAllAsReadAsync(int? workspaceId)
    {
        var query = _context.Notifications.Where(n => !n.IsRead);

        if (workspaceId.HasValue)
        {
            query = query.Where(n => n.WorkspaceId == workspaceId.Value);
        }

        var unreadNotifications = await query.ToListAsync();
        foreach (var n in unreadNotifications)
        {
            n.IsRead = true;
        }

        await _context.SaveChangesAsync();
        return unreadNotifications.Count;
    }

    public async Task<NotificationResponse> CreateNotificationAsync(int workspaceId, string title, string message, string type)
    {
        var notification = new Notification
        {
            WorkspaceId = workspaceId,
            Title = title.Trim(),
            Message = message.Trim(),
            Type = NotificationTypes.IsValid(type) ? type : NotificationTypes.Info,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return MapToResponse(notification);
    }

    private static NotificationResponse MapToResponse(Notification n)
    {
        return new NotificationResponse
        {
            Id = n.Id,
            WorkspaceId = n.WorkspaceId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        };
    }
}
