namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Sale;

public interface ISaleService
{
    Task<ApiResponse<SaleResponseDto>> CreateSaleAsync(CreateSaleDto saleDto, string userId);
    Task<ApiResponse<List<SaleResponseDto>>> GetAllSalesAsync(string userId);
    Task<ApiResponse<SaleResponseDto>> GetSaleByIdAsync(string id, string userId);
    Task<ApiResponse<bool>> DeleteSaleAsync(string id, string userId);
}