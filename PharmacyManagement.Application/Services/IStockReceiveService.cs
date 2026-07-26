using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;

namespace PharmacyManagement.Application.Services;

public interface IStockReceiveService
{
    Task<ApiResponse<ReceiveStockResponseDto>> CreateAsync(ReceiveStockDto dto, string userId);
    Task<ApiResponse<List<ReceiveStockResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<ReceiveStockResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<List<ReceiveStockResponseDto>>> GetBySupplierAsync(string supplierId, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
