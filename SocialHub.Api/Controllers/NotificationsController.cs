using Microsoft.AspNetCore.Mvc;
using SocialHub.Api.Common;
using SocialHub.Api.DTOs;
using SocialHub.Api.Interfaces;

namespace SocialHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get all notifications.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _notificationService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<NotificationResponse>>.Ok(notifications, "Notifications retrieved successfully."));
    }

    /// <summary>
    /// Get all notifications for a specific workspace.
    /// </summary>
    [HttpGet("workspace/{workspaceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var notifications = await _notificationService.GetByWorkspaceIdAsync(workspaceId);
        return Ok(ApiResponse<IEnumerable<NotificationResponse>>.Ok(notifications, "Workspace notifications retrieved successfully."));
    }

    /// <summary>
    /// Mark a notification as read.
    /// </summary>
    [HttpPut("{id:int}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _notificationService.MarkAsReadAsync(id);
        return Ok(ApiResponse<NotificationResponse>.Ok(notification, "Notification marked as read."));
    }

    /// <summary>
    /// Mark all notifications as read (optionally filtered by workspace).
    /// </summary>
    [HttpPut("read-all")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] int? workspaceId)
    {
        var count = await _notificationService.MarkAllAsReadAsync(workspaceId);
        return Ok(ApiResponse<int>.Ok(count, $"{count} notification(s) marked as read."));
    }
}
