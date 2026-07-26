using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Batch;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class BatchService : IBatchService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public BatchService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BatchResponseDto>> CreateAsync(BatchDto dto, string userId)
    {
        try
        {
            var drug = await _dbContext.Set<Drug>().FirstOrDefaultAsync(d => d.Id == dto.DrugId && d.UserId == userId);
            if (drug == null)
                return ApiResponse<BatchResponseDto>.ErrorResponse("Drug not found", statusCode: 404);

            var batch = _mapper.Map<Batch>(dto);
            batch.UserId = userId;
            batch.RemainingQuantity = dto.Quantity;
            batch.IsExpired = dto.ExpiryDate <= DateTime.UtcNow;

            _dbContext.Set<Batch>().Add(batch);

            _dbContext.Set<StockMovement>().Add(new StockMovement
            {
                DrugId = drug.Id,
                BatchId = batch.Id,
                Type = StockMovementType.PURCHASE,
                Quantity = dto.Quantity,
                RemainingAfter = dto.Quantity,
                UnitPrice = dto.PurchasePrice,
                TotalValue = dto.PurchasePrice * dto.Quantity,
                ReferenceId = batch.Id,
                ReferenceType = "batch",
                PerformedBy = userId,
                UserId = userId
            });

            if (drug.Status == DrugStatus.OUT_OF_STOCK && !batch.IsExpired)
            {
                drug.Status = DrugStatus.AVAILABLE;
            }

            await _dbContext.SaveChangesAsync();

            return ApiResponse<BatchResponseDto>.SuccessResponse(_mapper.Map<BatchResponseDto>(batch), "Batch created successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<BatchResponseDto>.ErrorResponse($"Failed to create batch: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BatchResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<Batch>()
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();

            return ApiResponse<List<BatchResponseDto>>.SuccessResponse(_mapper.Map<List<BatchResponseDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BatchResponseDto>>.ErrorResponse($"Failed to get batches: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BatchResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Batch>().FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (entity == null)
                return ApiResponse<BatchResponseDto>.ErrorResponse("Batch not found", statusCode: 404);

            return ApiResponse<BatchResponseDto>.SuccessResponse(_mapper.Map<BatchResponseDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<BatchResponseDto>.ErrorResponse($"Failed to get batch: {ex.Message}");
        }
    }

    public async Task<PagedResponse<BatchResponseDto>> GetByDrugAsync(string drugId, string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Set<Batch>().Where(b => b.DrugId == drugId && b.UserId == userId);
        var totalRecord = await query.CountAsync();

        var entities = await query
            .OrderBy(b => b.ExpiryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = _mapper.Map<List<BatchResponseDto>>(entities);
        return PagedResponse<BatchResponseDto>.Create(data, page, pageSize, totalRecord);
    }

    public async Task<ApiResponse<BatchResponseDto>> UpdateAsync(string id, UpdateBatchDto dto, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Batch>().FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (entity == null)
                return ApiResponse<BatchResponseDto>.ErrorResponse("Batch not found", statusCode: 404);

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<BatchResponseDto>.SuccessResponse(_mapper.Map<BatchResponseDto>(entity), "Batch updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<BatchResponseDto>.ErrorResponse($"Failed to update batch: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<Batch>().FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Batch not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Batch deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete batch: {ex.Message}");
        }
    }
}
