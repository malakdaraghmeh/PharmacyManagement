using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class StockReceiveService : IStockReceiveService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public StockReceiveService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReceiveStockResponseDto>> CreateAsync(ReceiveStockDto dto, string userId)
    {
        try
        {
            var supplier = await _dbContext.Set<Supplier>().FirstOrDefaultAsync(s => s.Id == dto.SupplierId && s.UserId == userId);
            if (supplier == null)
                return ApiResponse<ReceiveStockResponseDto>.ErrorResponse("Supplier not found", statusCode: 404);

            var receive = new StockReceive
            {
                SupplierId = dto.SupplierId,
                InvoiceNumber = dto.InvoiceNumber ?? string.Empty,
                Notes = dto.Notes ?? string.Empty,
                ReceivedAt = DateTime.UtcNow,
                UserId = userId
            };

            decimal totalCost = 0m;

            foreach (var itemDto in dto.Items)
            {
                var drug = await _dbContext.Set<Drug>().FirstOrDefaultAsync(d => d.Id == itemDto.DrugId && d.UserId == userId);
                if (drug == null)
                    return ApiResponse<ReceiveStockResponseDto>.ErrorResponse($"Drug {itemDto.DrugId} not found", statusCode: 404);

                var batch = new Batch
                {
                    DrugId = itemDto.DrugId,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Quantity = itemDto.Quantity,
                    RemainingQuantity = itemDto.Quantity,
                    PurchasePrice = itemDto.PurchasePrice,
                    SellingPrice = itemDto.SellingPrice,
                    SupplierId = dto.SupplierId,
                    ReceivedAt = DateTime.UtcNow,
                    IsExpired = itemDto.ExpiryDate <= DateTime.UtcNow,
                    UserId = userId
                };
                _dbContext.Set<Batch>().Add(batch);

                var subtotal = itemDto.PurchasePrice * itemDto.Quantity;
                totalCost += subtotal;

                receive.Items.Add(new StockReceiveItem
                {
                    DrugId = itemDto.DrugId,
                    DrugName = drug.Name,
                    BatchId = batch.Id,
                    BatchNumber = itemDto.BatchNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Quantity = itemDto.Quantity,
                    PurchasePrice = itemDto.PurchasePrice,
                    SellingPrice = itemDto.SellingPrice,
                    Subtotal = subtotal
                });

                _dbContext.Set<StockMovement>().Add(new StockMovement
                {
                    DrugId = itemDto.DrugId,
                    BatchId = batch.Id,
                    Type = StockMovementType.PURCHASE,
                    Quantity = itemDto.Quantity,
                    RemainingAfter = itemDto.Quantity,
                    UnitPrice = itemDto.PurchasePrice,
                    TotalValue = subtotal,
                    ReferenceId = receive.Id,
                    ReferenceType = "stock-receive",
                    PerformedBy = userId,
                    UserId = userId
                });

                if (drug.Status == DrugStatus.OUT_OF_STOCK && !batch.IsExpired)
                    drug.Status = DrugStatus.AVAILABLE;
            }

            receive.TotalCost = totalCost;
            _dbContext.Set<StockReceive>().Add(receive);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<ReceiveStockResponseDto>.SuccessResponse(await BuildResponseAsync(receive), "Stock received successfully", 201);
        }
        catch (Exception ex)
        {
            return ApiResponse<ReceiveStockResponseDto>.ErrorResponse($"Failed to receive stock: {ex.Message}");
        }
    }

    private async Task<ReceiveStockResponseDto> BuildResponseAsync(StockReceive receive)
    {
        var response = _mapper.Map<ReceiveStockResponseDto>(receive);
        response.SupplierName = await _dbContext.Set<Supplier>()
            .Where(s => s.Id == receive.SupplierId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync() ?? string.Empty;
        return response;
    }

    public async Task<ApiResponse<List<ReceiveStockResponseDto>>> GetAllAsync(string userId)
    {
        try
        {
            var entities = await _dbContext.Set<StockReceive>()
                .Include(r => r.Items)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReceivedAt)
                .ToListAsync();

            var list = new List<ReceiveStockResponseDto>();
            foreach (var e in entities)
                list.Add(await BuildResponseAsync(e));

            return ApiResponse<List<ReceiveStockResponseDto>>.SuccessResponse(list);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ReceiveStockResponseDto>>.ErrorResponse($"Failed to get stock receives: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ReceiveStockResponseDto>> GetByIdAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<StockReceive>()
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (entity == null)
                return ApiResponse<ReceiveStockResponseDto>.ErrorResponse("Stock receive not found", statusCode: 404);

            return ApiResponse<ReceiveStockResponseDto>.SuccessResponse(await BuildResponseAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiResponse<ReceiveStockResponseDto>.ErrorResponse($"Failed to get stock receive: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ReceiveStockResponseDto>>> GetBySupplierAsync(string supplierId, string userId)
    {
        try
        {
            var entities = await _dbContext.Set<StockReceive>()
                .Include(r => r.Items)
                .Where(r => r.SupplierId == supplierId && r.UserId == userId)
                .OrderByDescending(r => r.ReceivedAt)
                .ToListAsync();

            var list = new List<ReceiveStockResponseDto>();
            foreach (var e in entities)
                list.Add(await BuildResponseAsync(e));

            return ApiResponse<List<ReceiveStockResponseDto>>.SuccessResponse(list);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ReceiveStockResponseDto>>.ErrorResponse($"Failed to get stock receives: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, string userId)
    {
        try
        {
            var entity = await _dbContext.Set<StockReceive>().FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse("Stock receive not found", statusCode: 404);

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Stock receive deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete stock receive: {ex.Message}");
        }
    }
}
