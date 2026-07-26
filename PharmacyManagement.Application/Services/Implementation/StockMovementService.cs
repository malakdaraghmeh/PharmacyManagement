using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class StockMovementService : IStockMovementService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public StockMovementService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PagedResponse<StockMovementResponseDto>> GetAllAsync(string userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Set<StockMovement>().Where(m => m.UserId == userId);
        var totalRecord = await query.CountAsync();

        var entities = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = _mapper.Map<List<StockMovementResponseDto>>(entities);
        return PagedResponse<StockMovementResponseDto>.Create(data, page, pageSize, totalRecord);
    }

    public async Task<ApiResponse<List<StockMovementResponseDto>>> GetByDrugAsync(string drugId, string userId)
    {
        try
        {
            var entities = await _dbContext.Set<StockMovement>()
                .Where(m => m.DrugId == drugId && m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<StockMovementResponseDto>>.SuccessResponse(_mapper.Map<List<StockMovementResponseDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<StockMovementResponseDto>>.ErrorResponse($"Failed to get stock movements: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<StockMovementResponseDto>>> GetByBatchAsync(string batchId, string userId)
    {
        try
        {
            var entities = await _dbContext.Set<StockMovement>()
                .Where(m => m.BatchId == batchId && m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<StockMovementResponseDto>>.SuccessResponse(_mapper.Map<List<StockMovementResponseDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<StockMovementResponseDto>>.ErrorResponse($"Failed to get stock movements: {ex.Message}");
        }
    }
}
