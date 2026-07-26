using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Dashboard;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagement.Application.Services.Implementation;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    private async Task<Dictionary<string, int>> GetStockByDrugAsync(string userId)
    {
        return await _dbContext.Set<Batch>()
            .Where(b => b.UserId == userId)
            .GroupBy(b => b.DrugId)
            .Select(g => new { DrugId = g.Key, Total = g.Sum(x => x.RemainingQuantity) })
            .ToDictionaryAsync(x => x.DrugId, x => x.Total);
    }

    public async Task<ApiResponse<SalesTodayDto>> GetSalesTodayAsync(string userId)
    {
        try
        {
            var total = await _unitOfWork.Sales.GetTotalSalesTodayAsync(userId);
            var response = new SalesTodayDto { Total = total };
            return ApiResponse<SalesTodayDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<SalesTodayDto>.ErrorResponse($"Failed to get sales today: {ex.Message}");
        }
    }

    public async Task<ApiResponse<InvoicesTodayDto>> GetInvoicesTodayAsync(string userId)
    {
        try
        {
            var count = await _unitOfWork.Sales.GetInvoicesCountTodayAsync(userId);
            var response = new InvoicesTodayDto { Count = count };
            return ApiResponse<InvoicesTodayDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<InvoicesTodayDto>.ErrorResponse($"Failed to get invoices today: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<LowStockDrugDto>>> GetLowStockDrugsAsync(string userId)
    {
        try
        {
            var drugs = (await _unitOfWork.Drugs.GetByUserIdAsync(userId)).ToList();
            var stockByDrug = await GetStockByDrugAsync(userId);

            var response = drugs
                .Select(d => new { Drug = d, Qty = stockByDrug.TryGetValue(d.Id, out var t) ? t : 0 })
                .Where(x => x.Qty <= x.Drug.MinimumStock)
                .Select(x => new LowStockDrugDto
                {
                    Name = x.Drug.Name,
                    Quantity = x.Qty
                }).ToList();
            return ApiResponse<List<LowStockDrugDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<LowStockDrugDto>>.ErrorResponse($"Failed to get low stock drugs: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ExpiringDrugDto>>> GetExpiringDrugsAsync(string userId)
    {
        try
        {
            var thresholdDate = DateTime.UtcNow.AddDays(30);
            var response = await _dbContext.Set<Batch>()
                .Where(b => b.UserId == userId && b.RemainingQuantity > 0 && b.ExpiryDate <= thresholdDate)
                .Join(_dbContext.Set<Drug>(), b => b.DrugId, d => d.Id, (b, d) => new { d.Name, b.ExpiryDate })
                .ToListAsync();

            var result = response.Select(x => new ExpiringDrugDto
            {
                Name = x.Name,
                DaysLeft = (x.ExpiryDate - DateTime.UtcNow).Days
            }).ToList();
            return ApiResponse<List<ExpiringDrugDto>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ExpiringDrugDto>>.ErrorResponse($"Failed to get expiring drugs: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<AlertDto>>> GetAlertsAsync(string userId)
    {
        try
        {
            var alerts = new List<AlertDto>();

            var drugs = (await _unitOfWork.Drugs.GetByUserIdAsync(userId)).ToList();
            var stockByDrug = await GetStockByDrugAsync(userId);

            foreach (var d in drugs)
            {
                var qty = stockByDrug.TryGetValue(d.Id, out var t) ? t : 0;
                if (qty <= d.MinimumStock)
                {
                    alerts.Add(new AlertDto
                    {
                        Name = d.Name,
                        Message = $"Low stock: {qty} units remaining"
                    });
                }
                if (alerts.Count >= 5) break;
            }

            var thresholdDate = DateTime.UtcNow.AddDays(30);
            var expiring = await _dbContext.Set<Batch>()
                .Where(b => b.UserId == userId && b.RemainingQuantity > 0 && b.ExpiryDate <= thresholdDate)
                .Join(_dbContext.Set<Drug>(), b => b.DrugId, d => d.Id, (b, d) => new { d.Name, b.ExpiryDate })
                .Take(5)
                .ToListAsync();

            foreach (var e in expiring)
            {
                var daysLeft = (e.ExpiryDate - DateTime.UtcNow).Days;
                alerts.Add(new AlertDto
                {
                    Name = e.Name,
                    Message = $"Expires in {daysLeft} days"
                });
            }

            return ApiResponse<List<AlertDto>>.SuccessResponse(alerts);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AlertDto>>.ErrorResponse($"Failed to get alerts: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<RecentSaleDto>>> GetRecentSalesAsync(string userId)
    {
        try
        {
            var sales = await _unitOfWork.Sales.GetRecentSalesAsync(userId);
            var response = sales.Select(s => new RecentSaleDto
            {
                Time = s.CreatedAt.ToString("HH:mm"),
                Amount = s.NetAmount
            }).ToList();
            return ApiResponse<List<RecentSaleDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<RecentSaleDto>>.ErrorResponse($"Failed to get recent sales: {ex.Message}");
        }
    }
}