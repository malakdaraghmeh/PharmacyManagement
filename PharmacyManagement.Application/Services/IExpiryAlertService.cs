using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;

namespace PharmacyManagement.Application.Services;

public interface IExpiryAlertService
{
    Task<ApiResponse<List<ExpiryAlertDto>>> GetAllAsync(string userId);
    Task<ApiResponse<ExpiryAlertDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<List<ExpiryAlertDto>>> GetByDrugAsync(string drugId, string userId);
    Task<ApiResponse<ExpiryAlertDto>> AcknowledgeAsync(string id, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
