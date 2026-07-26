using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Supplier;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public SupplierService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SupplierResponseDto>> CreateAsync(SupplierDto dto, string userId)
    {
        try
        {
            var entity = _mapper.Map<Supplier>(dto);
            entity.UserId = userId;
            entity.IsActive = true;

            _dbContext.Set<Supplier>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<SupplierResponseDto>.SuccessResponse(_mapper.Map<SupplierResponseDto>(entity), "Supplier created successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<SupplierResponseDto>.ErrorResponse($"Failed to create supplier: {ex.Message}");
        }
    }

    public async Task<PagedResponse<SupplierResponseDto>> GetAllAsync(string userId, int page, int pageSize, string? name)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Set<Supplier>().Where(s => s.UserId == userId);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(s => s.Name.ToLower().Contains(name.ToLower()));

        var totalRecord = await query.CountAsync();

        var entities = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = _mapper.Map<List<SupplierResponseDto>>(entities);
        return PagedResponse<SupplierResponseDto>.Create(data, page, pageSize, totalRecord);
    }

    public async Task<ApiResponse<SupplierResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Supplier>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<SupplierResponseDto>.ErrorResponse("Supplier not found", statusCode: 404);

            return ApiResponse<SupplierResponseDto>.SuccessResponse(_mapper.Map<SupplierResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<SupplierResponseDto>.ErrorResponse($"Failed to get supplier: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SupplierResponseDto>> UpdateAsync(string id, UpdateSupplierDto dto, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Supplier>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<SupplierResponseDto>.ErrorResponse("Supplier not found", statusCode: 404);

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<SupplierResponseDto>.SuccessResponse(_mapper.Map<SupplierResponseDto>(entity), "Supplier updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SupplierResponseDto>.ErrorResponse($"Failed to update supplier: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Supplier>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Supplier not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Supplier deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete supplier: {ex.Message}");
        }
    }
}
