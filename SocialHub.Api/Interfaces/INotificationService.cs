using SocialHub.Api.DTOs;

namespace SocialHub.Api.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponse>> GetAllAsync();
    Task<IEnumerable<NotificationResponse>> GetByWorkspaceIdAsync(int workspaceId);
    Task<NotificationResponse> MarkAsReadAsync(int id);
    Task<int> MarkAllAsReadAsync(int? workspaceId);
    Task<NotificationResponse> CreateNotificationAsync(int workspaceId, string title, string message, string type);
}
