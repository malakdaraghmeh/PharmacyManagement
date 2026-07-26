using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Unit;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class UnitService : IUnitService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UnitService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UnitResponseDto>> CreateAsync(UnitDto dto, string userId)
    {
        try
        {
            var entity = _mapper.Map<Unit>(dto);
            entity.UserId = userId;
            entity.IsActive = true;

            _dbContext.Set<Unit>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<UnitResponseDto>.SuccessResponse(_mapper.Map<UnitResponseDto>(entity), "Unit created successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<UnitResponseDto>.ErrorResponse($"Failed to create unit: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<UnitResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<Unit>()
                .Where(u => u.UserId == userId)
                .OrderBy(u => u.Name)
                .ToListAsync();

            return ApiResponse<List<UnitResponseDto>>.SuccessResponse(_mapper.Map<List<UnitResponseDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<UnitResponseDto>>.ErrorResponse($"Failed to get units: {ex.Message}");
        }
    }

    public async Task<ApiResponse<UnitResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Unit>().FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
            if (entity == null)
                return ApiResponse<UnitResponseDto>.ErrorResponse("Unit not found", statusCode: 404);

            return ApiResponse<UnitResponseDto>.SuccessResponse(_mapper.Map<UnitResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<UnitResponseDto>.ErrorResponse($"Failed to get unit: {ex.Message}");
        }
    }

    public async Task<ApiResponse<UnitResponseDto>> UpdateAsync(string id, UpdateUnitDto dto, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Unit>().FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
            if (entity == null)
                return ApiResponse<UnitResponseDto>.ErrorResponse("Unit not found", statusCode: 404);

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<UnitResponseDto>.SuccessResponse(_mapper.Map<UnitResponseDto>(entity), "Unit updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<UnitResponseDto>.ErrorResponse($"Failed to update unit: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Unit>().FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Unit not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Unit deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete unit: {ex.Message}");
        }
    }
}
