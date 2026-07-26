using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;

namespace PharmacyManagement.Application.Services;

public interface ILowStockService
{
    Task<ApiResponse<List<LowStockDto>>> GetAllAsync(string userId);
    Task<ApiResponse<List<LowStockDto>>> GetCriticalAsync(string userId);
    Task<ApiResponse<LowStockDto>> GetByDrugAsync(string drugId, string userId);
    Task<ApiResponse<List<LowStockDto>>> RegenerateAsync(string userId);
}
