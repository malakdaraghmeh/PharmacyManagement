using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Supplier;

namespace PharmacyManagement.Application.Services;

public interface ISupplierService
{
    Task<ApiResponse<SupplierResponseDto>> CreateAsync(SupplierDto dto, string userId);
    Task<PagedResponse<SupplierResponseDto>> GetAllAsync(string userId, int page, int pageSize, string? name);
    Task<ApiResponse<SupplierResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<SupplierResponseDto>> UpdateAsync(string id, UpdateSupplierDto dto, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
