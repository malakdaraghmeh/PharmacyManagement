using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Settings;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _dbContext;

    public SettingsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<PharmacySettingsDto>> GetPharmacyAsync(string userId)
    {
        try
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<PharmacySettingsDto>.ErrorResponse("User not found", statusCode: 404);

            var dto = new PharmacySettingsDto
            {
                PharmacyName = user.PharmacyName,
                OwnerName = user.OwnerName,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                LogoUrl = user.LogoUrl
            };

            return ApiResponse<PharmacySettingsDto>.SuccessResponse(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PharmacySettingsDto>.ErrorResponse($"Failed to get pharmacy settings: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PharmacySettingsDto>> UpdatePharmacyAsync(UpdatePharmacySettingsDto dto, string userId)
    {
        try
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<PharmacySettingsDto>.ErrorResponse("User not found", statusCode: 404);

            user.PharmacyName = dto.PharmacyName;
            user.OwnerName = dto.OwnerName;
            user.PhoneNumber = dto.Phone;
            user.Email = dto.Email;
            user.Address = dto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return await GetPharmacyAsync(userId);
        }
        catch (Exception ex)
        {
            return ApiResponse<PharmacySettingsDto>.ErrorResponse($"Failed to update pharmacy settings: {ex.Message}");
        }
    }

    private async Task<SystemSettings> GetOrCreateSystemAsync(string userId)
    {
        var settings = await _dbContext.Set<SystemSettings>().FirstOrDefaultAsync(s => s.UserId == userId);
        if (settings == null)
        {
            settings = new SystemSettings
            {
                Currency = "USD",
                TaxPercentage = 0,
                EnableLowStockNotification = true,
                EnableExpiryNotification = true,
                ExpiryAlertDays = 90,
                MinimumPasswordLength = 6,
                UserId = userId
            };
            _dbContext.Set<SystemSettings>().Add(settings);
            await _dbContext.SaveChangesAsync();
        }
        return settings;
    }

    public async Task<ApiResponse<SystemSettingsDto>> GetSystemAsync(string userId)
    {
        try
        {
            var settings = await GetOrCreateSystemAsync(userId);
            return ApiResponse<SystemSettingsDto>.SuccessResponse(MapToDto(settings));
        }
        catch (Exception ex)
        {
            return ApiResponse<SystemSettingsDto>.ErrorResponse($"Failed to get system settings: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SystemSettingsDto>> UpdateSystemAsync(SystemSettingsDto dto, string userId)
    {
        try
        {
            var settings = await GetOrCreateSystemAsync(userId);
            settings.Currency = dto.Currency;
            settings.TaxPercentage = dto.TaxPercentage;
            settings.EnableLowStockNotification = dto.EnableLowStockNotification;
            settings.EnableExpiryNotification = dto.EnableExpiryNotification;
            settings.ExpiryAlertDays = dto.ExpiryAlertDays;
            settings.MinimumPasswordLength = dto.MinimumPasswordLength;
            settings.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<SystemSettingsDto>.SuccessResponse(MapToDto(settings), "System settings updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SystemSettingsDto>.ErrorResponse($"Failed to update system settings: {ex.Message}");
        }
    }

    private static SystemSettingsDto MapToDto(SystemSettings s) => new()
    {
        Currency = s.Currency,
        TaxPercentage = s.TaxPercentage,
        EnableLowStockNotification = s.EnableLowStockNotification,
        EnableExpiryNotification = s.EnableExpiryNotification,
        ExpiryAlertDays = s.ExpiryAlertDays,
        MinimumPasswordLength = s.MinimumPasswordLength
    };
}
