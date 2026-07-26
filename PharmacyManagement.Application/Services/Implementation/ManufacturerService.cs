using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Manufacturer;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class ManufacturerService : IManufacturerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ManufacturerService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ManufacturerResponseDto>> CreateAsync(ManufacturerDto dto, string userId)
    {
        try
        {
            var entity = _mapper.Map<Manufacturer>(dto);
            entity.UserId = userId;
            entity.IsActive = true;

            _dbContext.Set<Manufacturer>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<ManufacturerResponseDto>.SuccessResponse(_mapper.Map<ManufacturerResponseDto>(entity), "Manufacturer created successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<ManufacturerResponseDto>.ErrorResponse($"Failed to create manufacturer: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ManufacturerResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<Manufacturer>()
                .Where(m => m.UserId == userId)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return ApiResponse<List<ManufacturerResponseDto>>.SuccessResponse(_mapper.Map<List<ManufacturerResponseDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ManufacturerResponseDto>>.ErrorResponse($"Failed to get manufacturers: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ManufacturerResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Manufacturer>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (entity == null)
                return ApiResponse<ManufacturerResponseDto>.ErrorResponse("Manufacturer not found", statusCode: 404);

            return ApiResponse<ManufacturerResponseDto>.SuccessResponse(_mapper.Map<ManufacturerResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<ManufacturerResponseDto>.ErrorResponse($"Failed to get manufacturer: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ManufacturerResponseDto>> UpdateAsync(string id, ManufacturerDto dto, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Manufacturer>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (entity == null)
                return ApiResponse<ManufacturerResponseDto>.ErrorResponse("Manufacturer not found", statusCode: 404);

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<ManufacturerResponseDto>.SuccessResponse(_mapper.Map<ManufacturerResponseDto>(entity), "Manufacturer updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ManufacturerResponseDto>.ErrorResponse($"Failed to update manufacturer: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Manufacturer>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Manufacturer not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Manufacturer deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete manufacturer: {ex.Message}");
        }
    }
}
