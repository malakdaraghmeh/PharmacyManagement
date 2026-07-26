using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;

namespace PharmacyManagement.Application.Services;

public interface IStockAdjustmentService
{
    Task<ApiResponse<StockAdjustmentResponseDto>> CreateAsync(StockAdjustmentDto dto, string userId);
    Task<ApiResponse<List<StockAdjustmentResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<StockAdjustmentResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<StockAdjustmentResponseDto>> ApproveAsync(string id, string userId);
    Task<ApiResponse<StockAdjustmentResponseDto>> RejectAsync(string id, string userId);
}
