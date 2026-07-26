using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Inventory;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class ExpiryAlertService : IExpiryAlertService
{
    private const int NearExpiryThresholdDays = 90;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ExpiryAlertService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    private async Task RegenerateAsync(string userId)
    {
        var batches = await _dbContext.Set<Batch>()
            .Where(b => b.UserId == userId && b.RemainingQuantity > 0)
            .ToListAsync();

        var existingAlerts = await _dbContext.Set<ExpiryAlert>()
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var batch in batches)
        {
            var daysToExpire = (int)Math.Ceiling((batch.ExpiryDate - now).TotalDays);
            if (daysToExpire > NearExpiryThresholdDays)
                continue; // safe, no alert needed

            var status = daysToExpire <= 0 ? ExpiryStatus.EXPIRED : ExpiryStatus.NEAR_EXPIRY;
            var severity = daysToExpire <= 0 ? ExpirySeverity.CRITICAL
                : daysToExpire <= 7 ? ExpirySeverity.CRITICAL
                : daysToExpire <= 30 ? ExpirySeverity.HIGH
                : daysToExpire <= 60 ? ExpirySeverity.MEDIUM
                : ExpirySeverity.LOW;
            var action = daysToExpire <= 0 ? ExpiryAction.DISCARD
                : daysToExpire <= 30 ? ExpiryAction.PRIORITY_SALE
                : daysToExpire <= 60 ? ExpiryAction.DISCOUNT_SALE
                : ExpiryAction.NONE;
            var lossValue = batch.RemainingQuantity * batch.PurchasePrice;

            var alert = existingAlerts.FirstOrDefault(a => a.BatchId == batch.Id);
            if (alert == null)
            {
                _dbContext.Set<ExpiryAlert>().Add(new ExpiryAlert
                {
                    BatchId = batch.Id,
                    DrugId = batch.DrugId,
                    BatchNumber = batch.BatchNumber,
                    ExpiryDate = batch.ExpiryDate,
                    RemainingQuantity = batch.RemainingQuantity,
                    DaysToExpire = daysToExpire,
                    Status = status,
                    Severity = severity,
                    EstimatedLossValue = lossValue,
                    RecommendedAction = action,
                    IsAcknowledged = false,
                    UserId = userId
                });
            }
            else
            {
                alert.RemainingQuantity = batch.RemainingQuantity;
                alert.DaysToExpire = daysToExpire;
                alert.Status = status;
                alert.Severity = severity;
                alert.EstimatedLossValue = lossValue;
                alert.RecommendedAction = action;
                alert.UpdatedAt = now;
            }
        }

        // Remove alerts whose batch is depleted or no longer near expiry
        var validBatchIds = batches
            .Where(b => (int)Math.Ceiling((b.ExpiryDate - now).TotalDays) <= NearExpiryThresholdDays)
            .Select(b => b.Id)
            .ToHashSet();

        foreach (var alert in existingAlerts.Where(a => !validBatchIds.Contains(a.BatchId)))
        {
            alert.IsDeleted = true;
            alert.UpdatedAt = now;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ApiResponse<List<ExpiryAlertDto>>> GetAllAsync(string userId)
    {
        try
        {
            await RegenerateAsync(userId);
            var entities = await _dbContext.Set<ExpiryAlert>()
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.DaysToExpire)
                .ToListAsync();

            return ApiResponse<List<ExpiryAlertDto>>.SuccessResponse(_mapper.Map<List<ExpiryAlertDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ExpiryAlertDto>>.ErrorResponse($"Failed to get expiry alerts: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ExpiryAlertDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<ExpiryAlert>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (entity == null)
                return ApiResponse<ExpiryAlertDto>.ErrorResponse("Expiry alert not found", statusCode: 404);

            return ApiResponse<ExpiryAlertDto>.SuccessResponse(_mapper.Map<ExpiryAlertDto>(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<ExpiryAlertDto>.ErrorResponse($"Failed to get expiry alert: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ExpiryAlertDto>>> GetByDrugAsync(string drugId, string userId)
    {
        try
        {
            await RegenerateAsync(userId);
            var entities = await _dbContext.Set<ExpiryAlert>()
                .Where(a => a.DrugId == drugId && a.UserId == userId)
                .OrderBy(a => a.DaysToExpire)
                .ToListAsync();

            return ApiResponse<List<ExpiryAlertDto>>.SuccessResponse(_mapper.Map<List<ExpiryAlertDto>>(entities));
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ExpiryAlertDto>>.ErrorResponse($"Failed to get expiry alerts: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ExpiryAlertDto>> AcknowledgeAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<ExpiryAlert>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (entity == null)
                return ApiResponse<ExpiryAlertDto>.ErrorResponse("Expiry alert not found", statusCode: 404);

            entity.IsAcknowledged = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<ExpiryAlertDto>.SuccessResponse(_mapper.Map<ExpiryAlertDto>(entity), "Expiry alert acknowledged");
        }
        catch (Exception ex)
        {
            return ApiResponse<ExpiryAlertDto>.ErrorResponse($"Failed to acknowledge expiry alert: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<ExpiryAlert>().FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Expiry alert not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Expiry alert deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete expiry alert: {ex.Message}");
        }
    }
}
