using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Batch;

namespace PharmacyManagement.Application.Services;

public interface IBatchService
{
    Task<ApiResponse<BatchResponseDto>> CreateAsync(BatchDto dto, string userId);
    Task<ApiResponse<List<BatchResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<BatchResponseDto>> GetByIdAsync(string id, string userId);
    Task<PagedResponse<BatchResponseDto>> GetByDrugAsync(string drugId, string userId, int page, int pageSize);
    Task<ApiResponse<BatchResponseDto>> UpdateAsync(string id, UpdateBatchDto dto, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
