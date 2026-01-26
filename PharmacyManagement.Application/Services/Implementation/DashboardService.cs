using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Dashboard;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
            var drugs = await _unitOfWork.Drugs.GetLowStockDrugsAsync(userId);
            var response = drugs.Select(d => new LowStockDrugDto
            {
                Name = d.Name,
                Quantity = d.Quantity
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
            var drugs = await _unitOfWork.Drugs.GetExpiringDrugsAsync(userId);
            var response = drugs.Select(d => new ExpiringDrugDto
            {
                Name = d.Name,
                DaysLeft = (d.ExpiryDate - DateTime.UtcNow).Days
            }).ToList();
            return ApiResponse<List<ExpiringDrugDto>>.SuccessResponse(response);
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

            var lowStockDrugs = await _unitOfWork.Drugs.GetLowStockDrugsAsync(userId);
            foreach (var drug in lowStockDrugs.Take(5))
            {
                alerts.Add(new AlertDto
                {
                    Name = drug.Name,
                    Message = $"Low stock: {drug.Quantity} units remaining"
                });
            }

            var expiringDrugs = await _unitOfWork.Drugs.GetExpiringDrugsAsync(userId);
            foreach (var drug in expiringDrugs.Take(5))
            {
                var daysLeft = (drug.ExpiryDate - DateTime.UtcNow).Days;
                alerts.Add(new AlertDto
                {
                    Name = drug.Name,
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