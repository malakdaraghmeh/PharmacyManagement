using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Category;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryResponseDto>> CreateAsync(CategoryDto dto, string userId)
    {
        try
        {
            var entity = _mapper.Map<Category>(dto);
            entity.UserId = userId;
            entity.IsActive = true;

            _dbContext.Set<Category>().Add(entity);
            await _dbContext.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponseDto>(entity);
            return ApiResponse<CategoryResponseDto>.SuccessResponse(response, "Category created successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryResponseDto>.ErrorResponse($"Failed to create category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<Category>()
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var response = _mapper.Map<List<CategoryResponseDto>>(entities);
            return ApiResponse<List<CategoryResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CategoryResponseDto>>.ErrorResponse($"Failed to get categories: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CategoryResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Category not found", statusCode: 404);

            return ApiResponse<CategoryResponseDto>.SuccessResponse(_mapper.Map<CategoryResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryResponseDto>.ErrorResponse($"Failed to get category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CategoryResponseDto>> UpdateAsync(string id, UpdateCategoryDto dto, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
                return ApiResponse<CategoryResponseDto>.ErrorResponse("Category not found", statusCode: 404);

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<CategoryResponseDto>.SuccessResponse(_mapper.Map<CategoryResponseDto>(entity), "Category updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryResponseDto>.ErrorResponse($"Failed to update category: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Category not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete category: {ex.Message}");
        }
    }
}
