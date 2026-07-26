using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Category;

namespace PharmacyManagement.Application.Services;

public interface ICategoryService
{
    Task<ApiResponse<CategoryResponseDto>> CreateAsync(CategoryDto dto, string userId);
    Task<ApiResponse<List<CategoryResponseDto>>> GetAllAsync(string userId);
    Task<ApiResponse<CategoryResponseDto>> GetByIdAsync(string id, string userId);
    Task<ApiResponse<CategoryResponseDto>> UpdateAsync(string id, UpdateCategoryDto dto, string userId);
    Task<ApiResponse<bool>> DeleteAsync(string id, string userId);
}
