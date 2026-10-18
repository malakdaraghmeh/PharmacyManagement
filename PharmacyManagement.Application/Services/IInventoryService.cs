using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;

namespace PharmacyManagement.Application.Services;

public interface IInventoryService
{
    Task<ApiResponse<InventorySummaryDto>> GetSummaryAsync(string userId);
    Task<PagedResponse<LowStockDto>> GetLowStockItemsAsync(string userId, int page, int pageSize);
    Task<PagedResponse<ExpiryAlertDto>> GetExpiryAlertsAsync(string userId, int page, int pageSize);
    Task<PagedResponse<TopMovingDrugDto>> GetTopMoversAsync(string userId, int page, int pageSize);
    Task<PagedResponse<InventoryRecommendationDto>> GetRecommendationsAsync(string userId, int page, int pageSize);
    Task<ApiResponse<double>> GetRiskScoreAsync(string userId);
}
