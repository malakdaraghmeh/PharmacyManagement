using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;

namespace PharmacyManagement.Application.Services;

public interface IStockMovementService
{
    Task<PagedResponse<StockMovementResponseDto>> GetAllAsync(string userId, int page, int pageSize);
    Task<ApiResponse<List<StockMovementResponseDto>>> GetByDrugAsync(string drugId, string userId);
    Task<ApiResponse<List<StockMovementResponseDto>>> GetByBatchAsync(string batchId, string userId);
}
