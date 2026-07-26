using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Manufacturer;

namespace PharmacyManagement.Application.Services;

public interface IManufacturerService
{
    Task<ApiResponse<ManufacturerResponseDto>> CreateAsync(ManufacturerDto dto, string userId);
    Task<ApiResponse<List<ManufacturerResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<ManufacturerResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<ManufacturerResponseDto>> UpdateAsync(string id, ManufacturerDto dto, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
