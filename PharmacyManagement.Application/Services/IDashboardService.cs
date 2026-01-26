namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Dashboard;

public interface IDashboardService
{
    Task<ApiResponse<SalesTodayDto>> GetSalesTodayAsync(string userId);
    Task<ApiResponse<InvoicesTodayDto>> GetInvoicesTodayAsync(string userId);
    Task<ApiResponse<List<LowStockDrugDto>>> GetLowStockDrugsAsync(string userId);
    Task<ApiResponse<List<ExpiringDrugDto>>> GetExpiringDrugsAsync(string userId);
    Task<ApiResponse<List<AlertDto>>> GetAlertsAsync(string userId);
    Task<ApiResponse<List<RecentSaleDto>>> GetRecentSalesAsync(string userId);
}