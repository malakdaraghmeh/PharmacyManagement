using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;

namespace PharmacyManagement.Application.Services;

public interface IInventoryService
{
    Task<ApiResponse<InventorySummaryDto>> GetSummaryAsync(string userId);
    Task<ApiResponse<List<TopMovingDrugDto>>> GetTopMoversAsync(string userId);
    Task<ApiResponse<List<InventoryRecommendationDto>>> GetRecommendationsAsync(string userId);
    Task<ApiResponse<double>> GetRiskScoreAsync(string userId);
}
