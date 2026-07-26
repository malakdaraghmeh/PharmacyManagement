using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class StockAdjustmentService : IStockAdjustmentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public StockAdjustmentService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    private async Task<int> GetDrugStockAsync(string drugId, string userId)
    {
        return await _dbContext.Set<Batch>()
            .Where(b => b.DrugId == drugId && b.UserId == userId)
            .SumAsync(b => b.RemainingQuantity);
    }

    private static int ComputeQuantityAfter(StockAdjustmentType type, int before, int adjustment)
    {
        return type switch
        {
            StockAdjustmentType.INCREASE => before + adjustment,
            StockAdjustmentType.COUNT_CORRECTION => adjustment,
            _ => Math.Max(0, before - adjustment) // DECREASE, DAMAGE, LOSS
        };
    }

    private async Task<StockAdjustmentResponseDto> BuildResponseAsync(StockAdjustment entity)
    {
        var response = _mapper.Map<StockAdjustmentResponseDto>(entity);
        response.DrugName = await _dbContext.Set<Drug>()
            .Where(d => d.Id == entity.DrugId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync() ?? string.Empty;
        if (!string.IsNullOrEmpty(entity.BatchId))
        {
            response.BatchNumber = await _dbContext.Set<Batch>()
                .Where(b => b.Id == entity.BatchId)
                .Select(b => b.BatchNumber)
                .FirstOrDefaultAsync();
        }
        return response;
    }

    public async Task<ApiResponse<StockAdjustmentResponseDto>> CreateAsync(StockAdjustmentDto dto, string userId)
    {
        try
        {
            var drug = await _dbContext.Set<Drug>().FirstOrDefaultAsync(d => d.Id == dto.DrugId && d.UserId == userId);
            if (drug == null)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Drug not found", statusCode: 404);

            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);

            var before = await GetDrugStockAsync(dto.DrugId, userId);
            var after = ComputeQuantityAfter(dto.Type, before, dto.AdjustmentQuantity);

            var entity = new StockAdjustment
            {
                DrugId = dto.DrugId,
                BatchId = dto.BatchId,
                Type = dto.Type,
                QuantityBefore = before,
                QuantityAfter = after,
                AdjustmentQuantity = dto.AdjustmentQuantity,
                Reason = dto.Reason,
                Status = AdjustmentStatus.PENDING,
                RequestedBy = userId,
                RequestedByName = user?.OwnerName ?? user?.PharmacyName ?? string.Empty,
                UserId = userId
            };

            _dbContext.Set<StockAdjustment>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<StockAdjustmentResponseDto>.SuccessResponse(await BuildResponseAsync(entity), "Stock adjustment requested", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse($"Failed to create stock adjustment: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<StockAdjustmentResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<StockAdjustment>()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var list = new List<StockAdjustmentResponseDto>();
            foreach (var e in entities)
                list.Add(await BuildResponseAsync(e));

            return ApiResponse<List<StockAdjustmentResponseDto>>.SuccessResponse(list);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<StockAdjustmentResponseDto>>.ErrorResponse($"Failed to get stock adjustments: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StockAdjustmentResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<StockAdjustment>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Stock adjustment not found", statusCode: 404);

            return ApiResponse<StockAdjustmentResponseDto>.SuccessResponse(await BuildResponseAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse($"Failed to get stock adjustment: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StockAdjustmentResponseDto>> ApproveAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<StockAdjustment>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Stock adjustment not found", statusCode: 404);

            if (entity.Status != AdjustmentStatus.PENDING)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Only pending adjustments can be approved");

            await ApplyAdjustmentAsync(entity, userId);

            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
            entity.Status = AdjustmentStatus.APPROVED;
            entity.ApprovedBy = userId;
            entity.ApprovedByName = user?.OwnerName ?? user?.PharmacyName ?? string.Empty;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<StockAdjustmentResponseDto>.SuccessResponse(await BuildResponseAsync(entity), "Stock adjustment approved");
        }
        catch (Exception ex)
        {
            return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse($"Failed to approve stock adjustment: {ex.Message}");
        }
    }

    public async Task<ApiResponse<StockAdjustmentResponseDto>> RejectAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<StockAdjustment>().FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (entity == null)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Stock adjustment not found", statusCode: 404);

            if (entity.Status != AdjustmentStatus.PENDING)
                return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse("Only pending adjustments can be rejected");

            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
            entity.Status = AdjustmentStatus.REJECTED;
            entity.ApprovedBy = userId;
            entity.ApprovedByName = user?.OwnerName ?? user?.PharmacyName ?? string.Empty;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return ApiResponse<StockAdjustmentResponseDto>.SuccessResponse(await BuildResponseAsync(entity), "Stock adjustment rejected");
        }
        catch (Exception ex)
        {
            return ApiResponse<StockAdjustmentResponseDto>.ErrorResponse($"Failed to reject stock adjustment: {ex.Message}");
        }
    }

    private async Task ApplyAdjustmentAsync(StockAdjustment entity, string userId)
    {
        var isIncrease = entity.Type == StockAdjustmentType.INCREASE
            || (entity.Type == StockAdjustmentType.COUNT_CORRECTION && entity.QuantityAfter >= entity.QuantityBefore);

        var delta = Math.Abs(entity.QuantityAfter - entity.QuantityBefore);
        if (delta == 0) return;

        var batches = await _dbContext.Set<Batch>()
            .Where(b => b.DrugId == entity.DrugId && b.UserId == userId
                && (string.IsNullOrEmpty(entity.BatchId) || b.Id == entity.BatchId))
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();

        if (isIncrease)
        {
            var target = batches.LastOrDefault();
            if (target != null)
            {
                target.RemainingQuantity += delta;
                RecordMovement(entity, target, delta, target.RemainingQuantity, userId);
            }
        }
        else
        {
            var remaining = delta;
            foreach (var batch in batches)
            {
                if (remaining <= 0) break;
                var take = Math.Min(batch.RemainingQuantity, remaining);
                batch.RemainingQuantity -= take;
                remaining -= take;
                RecordMovement(entity, batch, -take, batch.RemainingQuantity, userId);
            }
        }
    }

    private void RecordMovement(StockAdjustment entity, Batch batch, int quantity, int remainingAfter, string userId)
    {
        _dbContext.Set<StockMovement>().Add(new StockMovement
        {
            DrugId = entity.DrugId,
            BatchId = batch.Id,
            Type = StockMovementType.ADJUSTMENT,
            Quantity = quantity,
            RemainingAfter = remainingAfter,
            UnitPrice = batch.PurchasePrice,
            TotalValue = batch.PurchasePrice * Math.Abs(quantity),
            ReferenceId = entity.Id,
            ReferenceType = "adjustment",
            PerformedBy = userId,
            Reason = entity.Reason,
            UserId = userId
        });
    }
}
