namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Notification;

public interface INotificationService
{
    Task<ApiResponse<List<NotificationResponseDto>>> GetNotificationsAsync(string userId);
    Task<ApiResponse<object>> MarkAsReadAsync(int id, string userId);
    Task CreateNotificationAsync(string userId, string title, string message, string type);
}