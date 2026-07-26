using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Settings;

namespace PharmacyManagement.Application.Services;

public interface ISettingsService
{
    Task<ApiResponse<PharmacySettingsDto>> GetPharmacyAsync(string userId);
    Task<ApiResponse<PharmacySettingsDto>> UpdatePharmacyAsync(UpdatePharmacySettingsDto dto, string userId);
    Task<ApiResponse<SystemSettingsDto>> GetSystemAsync(string userId);
    Task<ApiResponse<SystemSettingsDto>> UpdateSystemAsync(SystemSettingsDto dto, string userId);
}
