using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Notification;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<NotificationResponseDto>>> GetNotificationsAsync(string userId)
    {
        try
        {
            var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
            var response = _mapper.Map<List<NotificationResponseDto>>(notifications);
            return ApiResponse<List<NotificationResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<NotificationResponseDto>>.ErrorResponse($"Failed to get notifications: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> MarkAsReadAsync(int id, string userId)
    {
        try
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id.ToString());

            if (notification == null || notification.UserId != userId)
            {
                return ApiResponse<object>.ErrorResponse("Notification not found");
            }

            await _unitOfWork.Notifications.MarkAsReadAsync(id.ToString());
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<object>.SuccessResponse(new { }, "Notification marked as read");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.ErrorResponse($"Failed to mark notification as read: {ex.Message}");
        }
    }

    public async Task CreateNotificationAsync(string userId, string title, string message, string type)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            // Log error but don't throw
        }
    }
}