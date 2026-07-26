using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class LowStockService : ILowStockService
{
    private readonly ApplicationDbContext _dbContext;

    public LowStockService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<List<LowStockDto>> ComputeAsync(string userId, string? drugId = null)
    {
        var drugsQuery = _dbContext.Set<Drug>().Where(d => d.UserId == userId);
        if (!string.IsNullOrEmpty(drugId))
            drugsQuery = drugsQuery.Where(d => d.Id == drugId);

        var drugs = await drugsQuery.ToListAsync();
        var drugIds = drugs.Select(d => d.Id).ToList();

        var batches = await _dbContext.Set<Batch>()
            .Where(b => b.UserId == userId && drugIds.Contains(b.DrugId))
            .ToListAsync();

        var since = DateTime.UtcNow.AddDays(-30);
        var saleMovements = await _dbContext.Set<StockMovement>()
            .Where(m => m.UserId == userId && m.Type == StockMovementType.SALE && m.CreatedAt >= since && drugIds.Contains(m.DrugId))
            .ToListAsync();

        var allSaleMovements = await _dbContext.Set<StockMovement>()
            .Where(m => m.UserId == userId && m.Type == StockMovementType.SALE && drugIds.Contains(m.DrugId))
            .ToListAsync();

        var result = new List<LowStockDto>();

        foreach (var drug in drugs)
        {
            var totalStock = batches.Where(b => b.DrugId == drug.Id).Sum(b => b.RemainingQuantity);
            var soldLast30 = saleMovements.Where(m => m.DrugId == drug.Id).Sum(m => Math.Abs(m.Quantity));
            var avgDaily = soldLast30 / 30.0;
            var estimatedDaysLeft = avgDaily > 0 ? Math.Round(totalStock / avgDaily, 1) : 0;

            var status = totalStock <= 0
                ? LowStockStatus.OUT_OF_STOCK
                : totalStock <= drug.MinimumStock * 0.5
                    ? LowStockStatus.CRITICAL
                    : totalStock <= drug.MinimumStock
                        ? LowStockStatus.LOW
                        : LowStockStatus.OK;

            var severity = status switch
            {
                LowStockStatus.OUT_OF_STOCK => LowStockSeverity.URGENT,
                LowStockStatus.CRITICAL => LowStockSeverity.URGENT,
                LowStockStatus.LOW => LowStockSeverity.WARNING,
                _ => LowStockSeverity.NONE
            };

            var lastSale = allSaleMovements.Where(m => m.DrugId == drug.Id)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => (DateTime?)m.CreatedAt)
                .FirstOrDefault();

            var lastRestock = batches.Where(b => b.DrugId == drug.Id)
                .OrderByDescending(b => b.ReceivedAt)
                .Select(b => (DateTime?)b.ReceivedAt)
                .FirstOrDefault();

            var reorderQty = Math.Max(0, drug.MinimumStock * 2 - totalStock);

            result.Add(new LowStockDto
            {
                DrugId = drug.Id,
                DrugName = drug.Name,
                Barcode = drug.Barcode,
                TotalStock = totalStock,
                MinimumStock = drug.MinimumStock,
                Status = status,
                Severity = severity,
                AverageDailySales = Math.Round(avgDaily, 2),
                EstimatedDaysLeft = estimatedDaysLeft,
                RecommendedReorderQuantity = reorderQty,
                LastSaleDate = lastSale,
                LastRestockDate = lastRestock
            });
        }

        return result;
    }

    public async Task<ApiResponse<List<LowStockDto>>> GetAllAsync(string userId)
    {
        try
        {
            var all = await ComputeAsync(userId);
            var lowOnly = all.Where(x => x.Status != LowStockStatus.OK).ToList();
            return ApiResponse<List<LowStockDto>>.SuccessResponse(lowOnly);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LowStockDto>>.ErrorResponse($"Failed to get low stock: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LowStockDto>>> GetCriticalAsync(string userId)
    {
        try
        {
            var all = await ComputeAsync(userId);
            var critical = all.Where(x => x.Status == LowStockStatus.CRITICAL || x.Status == LowStockStatus.OUT_OF_STOCK).ToList();
            return ApiResponse<List<LowStockDto>>.SuccessResponse(critical);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LowStockDto>>.ErrorResponse($"Failed to get critical low stock: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LowStockDto>> GetByDrugAsync(string drugId, string userId)
    {
        try
        {
            var result = await ComputeAsync(userId, drugId);
            var item = result.FirstOrDefault();
            if (item == null)
                return ApiResponse<LowStockDto>.ErrorResponse("Drug not found", statusCode: 404);

            return ApiResponse<LowStockDto>.SuccessResponse(item);
        }
        catch (Exception ex)
        {
            return ApiResponse<LowStockDto>.ErrorResponse($"Failed to get low stock: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LowStockDto>>> RegenerateAsync(string userId)
    {
        try
        {
            var all = await ComputeAsync(userId);
            var lowOnly = all.Where(x => x.Status != LowStockStatus.OK).ToList();
            return ApiResponse<List<LowStockDto>>.SuccessResponse(lowOnly, "Low stock alerts regenerated");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LowStockDto>>.ErrorResponse($"Failed to regenerate low stock: {ex.Message}");
        }
    }
}
