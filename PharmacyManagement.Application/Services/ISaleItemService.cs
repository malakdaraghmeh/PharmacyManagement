namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Sale;

public interface ISaleItemService
{
    Task<ApiResponse<SaleItemResponseDto>> CreateSaleItemAsync(SaleItemDto saleItemDto, string userId);
    Task<ApiResponse<List<SaleItemResponseDto>>> GetAllSaleItemsAsync(string userId);
    Task<ApiResponse<SaleItemResponseDto>> GetSaleItemByIdAsync(string id, string userId);
    Task<ApiResponse<SaleItemResponseDto>> UpdateSaleItemAsync(string id, SaleItemDto saleItemDto, string userId);
    Task<ApiResponse<bool>> DeleteSaleItemAsync(string id, string userId);
}