using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILowStockService _lowStockService;
    private readonly IExpiryAlertService _expiryAlertService;

    public InventoryService(
        ApplicationDbContext dbContext,
        ILowStockService lowStockService,
        IExpiryAlertService expiryAlertService)
    {
        _dbContext = dbContext;
        _lowStockService = lowStockService;
        _expiryAlertService = expiryAlertService;
    }

    private async Task<List<TopMovingDrugDto>> ComputeTopMoversAsync(string userId)
    {
        var now = DateTime.UtcNow;
        var since = now.AddDays(-30);
        var midpoint = now.AddDays(-15);

        var movements = await _dbContext.Set<StockMovement>()
            .Where(m => m.UserId == userId && m.Type == StockMovementType.SALE && m.CreatedAt >= since)
            .ToListAsync();

        var drugNames = await _dbContext.Set<Drug>()
            .Where(d => d.UserId == userId)
            .ToDictionaryAsync(d => d.Id, d => d.Name);

        return movements
            .GroupBy(m => m.DrugId)
            .Select(g =>
            {
                var totalSold = g.Sum(x => Math.Abs(x.Quantity));
                var recent = g.Where(x => x.CreatedAt >= midpoint).Sum(x => Math.Abs(x.Quantity));
                var older = g.Where(x => x.CreatedAt < midpoint).Sum(x => Math.Abs(x.Quantity));
                var trend = recent > older ? "up" : recent < older ? "down" : "stable";
                return new TopMovingDrugDto
                {
                    DrugId = g.Key,
                    DrugName = drugNames.TryGetValue(g.Key, out var n) ? n : string.Empty,
                    TotalSold = totalSold,
                    Trend = trend,
                    DemandScore = Math.Round(totalSold / 30.0, 2)
                };
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(10)
            .ToList();
    }

    private async Task<double> ComputeRiskScoreAsync(string userId, List<LowStockDto> lowStock, List<ExpiryAlertDto> expiryAlerts)
    {
        var totalDrugs = await _dbContext.Set<Drug>().CountAsync(d => d.UserId == userId);
        if (totalDrugs == 0) return 0;

        var outOfStock = lowStock.Count(x => x.Status == LowStockStatus.OUT_OF_STOCK);
        var critical = lowStock.Count(x => x.Status == LowStockStatus.CRITICAL);
        var expired = expiryAlerts.Count(x => x.Status == ExpiryStatus.EXPIRED);
        var nearExpiry = expiryAlerts.Count(x => x.Status == ExpiryStatus.NEAR_EXPIRY);

        var score = (outOfStock * 3.0 + critical * 2.0 + expired * 3.0 + nearExpiry * 1.0)
            / (totalDrugs * 3.0) * 100.0;

        return Math.Round(Math.Min(100, score), 1);
    }

    public async Task<ApiResponse<InventorySummaryDto>> GetSummaryAsync(string userId)
    {
        try
        {
            var totalDrugs = await _dbContext.Set<Drug>().CountAsync(d => d.UserId == userId);
            var batches = await _dbContext.Set<Batch>().Where(b => b.UserId == userId).ToListAsync();

            var lowStock = (await _lowStockService.GetAllAsync(userId)).Data ?? new List<LowStockDto>();
            var expiryAlerts = (await _expiryAlertService.GetAllAsync(userId)).Data ?? new List<ExpiryAlertDto>();
            var topMovers = await ComputeTopMoversAsync(userId);
            var recommendations = await ComputeRecommendationsAsync(userId, lowStock, expiryAlerts);
            var riskScore = await ComputeRiskScoreAsync(userId, lowStock, expiryAlerts);

            var summary = new InventorySummaryDto
            {
                TotalDrugs = totalDrugs,
                TotalBatches = batches.Count,
                TotalStockItems = batches.Sum(b => b.RemainingQuantity),
                TotalInventoryValue = batches.Sum(b => b.RemainingQuantity * b.SellingPrice),
                LowStockItems = lowStock,
                ExpiryAlerts = expiryAlerts,
                OutOfStockDrugsCount = lowStock.Count(x => x.Status == LowStockStatus.OUT_OF_STOCK),
                CriticalAlertsCount = lowStock.Count(x => x.Status == LowStockStatus.CRITICAL)
                    + expiryAlerts.Count(x => x.Status == ExpiryStatus.EXPIRED),
                InventoryRiskScore = riskScore,
                TopMovingDrugs = topMovers,
                RecommendedActions = recommendations
            };

            return ApiResponse<InventorySummaryDto>.SuccessResponse(summary);
        }
        catch (Exception ex)
        {
            return ApiResponse<InventorySummaryDto>.ErrorResponse($"Failed to get inventory summary: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<TopMovingDrugDto>>> GetTopMoversAsync(string userId)
    {
        try
        {
            return ApiResponse<List<TopMovingDrugDto>>.SuccessResponse(await ComputeTopMoversAsync(userId));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<TopMovingDrugDto>>.ErrorResponse($"Failed to get top movers: {ex.Message}");
        }
    }

    private async Task<List<InventoryRecommendationDto>> ComputeRecommendationsAsync(string userId, List<LowStockDto> lowStock, List<ExpiryAlertDto> expiryAlerts)
    {
        var recommendations = new List<InventoryRecommendationDto>();

        foreach (var item in lowStock.Where(x => x.Status == LowStockStatus.OUT_OF_STOCK || x.Status == LowStockStatus.CRITICAL))
        {
            recommendations.Add(new InventoryRecommendationDto
            {
                Type = RecommendationType.REORDER,
                DrugId = item.DrugId,
                DrugName = item.DrugName,
                Message = $"Reorder {item.DrugName}. Current stock: {item.TotalStock}, recommended quantity: {item.RecommendedReorderQuantity}.",
                Priority = item.Status == LowStockStatus.OUT_OF_STOCK ? RecommendationPriority.CRITICAL : RecommendationPriority.HIGH
            });
        }

        foreach (var alert in expiryAlerts)
        {
            if (alert.Status == ExpiryStatus.EXPIRED)
            {
                recommendations.Add(new InventoryRecommendationDto
                {
                    Type = RecommendationType.REMOVE_EXPIRY,
                    DrugId = alert.DrugId,
                    Message = $"Remove expired batch {alert.BatchNumber}. Estimated loss: {alert.EstimatedLossValue}.",
                    Priority = RecommendationPriority.HIGH,
                    EstimatedImpact = alert.EstimatedLossValue
                });
            }
            else if (alert.Severity == ExpirySeverity.HIGH || alert.Severity == ExpirySeverity.CRITICAL)
            {
                recommendations.Add(new InventoryRecommendationDto
                {
                    Type = RecommendationType.DISCOUNT,
                    DrugId = alert.DrugId,
                    Message = $"Prioritize sale of batch {alert.BatchNumber} expiring in {alert.DaysToExpire} days.",
                    Priority = RecommendationPriority.MEDIUM,
                    EstimatedImpact = alert.EstimatedLossValue
                });
            }
        }

        await Task.CompletedTask;
        return recommendations;
    }

    public async Task<ApiResponse<List<InventoryRecommendationDto>>> GetRecommendationsAsync(string userId)
    {
        try
        {
            var lowStock = (await _lowStockService.GetAllAsync(userId)).Data ?? new List<LowStockDto>();
            var expiryAlerts = (await _expiryAlertService.GetAllAsync(userId)).Data ?? new List<ExpiryAlertDto>();
            var recommendations = await ComputeRecommendationsAsync(userId, lowStock, expiryAlerts);
            return ApiResponse<List<InventoryRecommendationDto>>.SuccessResponse(recommendations);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<InventoryRecommendationDto>>.ErrorResponse($"Failed to get recommendations: {ex.Message}");
        }
    }

    public async Task<ApiResponse<double>> GetRiskScoreAsync(string userId)
    {
        try
        {
            var lowStock = (await _lowStockService.GetAllAsync(userId)).Data ?? new List<LowStockDto>();
            var expiryAlerts = (await _expiryAlertService.GetAllAsync(userId)).Data ?? new List<ExpiryAlertDto>();
            var score = await ComputeRiskScoreAsync(userId, lowStock, expiryAlerts);
            return ApiResponse<double>.SuccessResponse(score);
        }
        catch (Exception ex)
        {
            return ApiResponse<double>.ErrorResponse($"Failed to get risk score: {ex.Message}");
        }
    }
}
