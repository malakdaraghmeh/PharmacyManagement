using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Unit;

namespace PharmacyManagement.Application.Services;

public interface IUnitService
{
    Task<ApiResponse<UnitResponseDto>> CreateAsync(UnitDto dto, string userId);
    Task<ApiResponse<List<UnitResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<UnitResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<UnitResponseDto>> UpdateAsync(string id, UpdateUnitDto dto, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
