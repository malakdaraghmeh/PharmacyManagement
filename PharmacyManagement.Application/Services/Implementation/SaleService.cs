using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

   private readonly ApplicationDbContext _dbContext;

public SaleService(
    IUnitOfWork unitOfWork, 
    IMapper mapper, 
    INotificationService notificationService,
    ApplicationDbContext dbContext)
{
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _notificationService = notificationService;
    _dbContext = dbContext;
}

    public async Task<ApiResponse<SaleResponseDto>> CreateSaleAsync(CreateSaleDto saleDto, string userId)
{
var strategy = _dbContext.Database.CreateExecutionStrategy();

return await strategy.ExecuteAsync<ApplicationDbContext, ApiResponse<SaleResponseDto>>(
    _dbContext, // state object
    async (db, state, ct) =>
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // --- Create sale ---
            var sale = _mapper.Map<Sale>(saleDto);
            sale.UserId = userId;
            sale.InvoiceNumber = GenerateInvoiceNumber();

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            foreach (var itemDto in saleDto.Items)
            {
                var drug = await _unitOfWork.Drugs.GetByIdAsync(itemDto.DrugId);
                if (drug == null || drug.UserId != userId)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<SaleResponseDto>.ErrorResponse($"Drug {itemDto.DrugName} not found");
                }

                // Fetch available batches for this drug ordered FEFO (First-Expiry-First-Out)
                var batches = await _dbContext.Set<Batch>()
                    .Where(b => b.DrugId == drug.Id && b.UserId == userId && b.RemainingQuantity > 0 && !b.IsExpired)
                    .OrderBy(b => b.ExpiryDate)
                    .ToListAsync();

                var totalAvailable = batches.Sum(b => b.RemainingQuantity);
                if (totalAvailable < itemDto.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<SaleResponseDto>.ErrorResponse($"Insufficient quantity for {itemDto.DrugName}");
                }

                var remainingToDeduct = itemDto.Quantity;
                foreach (var batch in batches)
                {
                    if (remainingToDeduct <= 0) break;

                    var deduct = Math.Min(batch.RemainingQuantity, remainingToDeduct);
                    batch.RemainingQuantity -= deduct;
                    remainingToDeduct -= deduct;

                    _dbContext.Set<StockMovement>().Add(new StockMovement
                    {
                        DrugId = drug.Id,
                        BatchId = batch.Id,
                        Type = StockMovementType.SALE,
                        Quantity = deduct,
                        RemainingAfter = batch.RemainingQuantity,
                        UnitPrice = itemDto.UnitPrice,
                        TotalValue = itemDto.UnitPrice * deduct,
                        ReferenceId = sale.Id,
                        ReferenceType = "sale",
                        PerformedBy = userId,
                        UserId = userId
                    });
                }

                var newTotal = totalAvailable - itemDto.Quantity;
                if (newTotal <= 0)
                {
                    drug.Status = DrugStatus.OUT_OF_STOCK;
                    await _unitOfWork.Drugs.UpdateAsync(drug);
                }

                if (newTotal <= drug.MinimumStock)
                {
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        "Low Stock Alert",
                        $"{drug.Name} is running low. Current quantity: {newTotal}",
                        "LowStock"
                    );
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var createdSale = await _unitOfWork.Sales.GetByIdWithItemsAsync(sale.Id);
            var response = _mapper.Map<SaleResponseDto>(createdSale);
            return ApiResponse<SaleResponseDto>.SuccessResponse(response, "Sale created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<SaleResponseDto>.ErrorResponse($"Failed to create sale: {ex.Message}");
        }
    },
    null, // no recovery operation needed
    default // cancellation token
);



}


    public async Task<ApiResponse<List<SaleResponseDto>>> GetAllSalesAsync(string userId)
    {
        try
        {
            var sales = await _unitOfWork.Sales.GetByUserIdAsync(userId);
            var response = _mapper.Map<List<SaleResponseDto>>(sales);
            return ApiResponse<List<SaleResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SaleResponseDto>>.ErrorResponse($"Failed to get sales: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SaleResponseDto>> GetSaleByIdAsync(string id, string userId)
    {
        try
        {
            var sale = await _unitOfWork.Sales.GetByIdWithItemsAsync(id);

            if (sale == null || sale.UserId != userId)
            {
                return ApiResponse<SaleResponseDto>.ErrorResponse("Sale not found");
            }

            var response = _mapper.Map<SaleResponseDto>(sale);
            return ApiResponse<SaleResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<SaleResponseDto>.ErrorResponse($"Failed to get sale: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSaleAsync(string id, string userId)
    {
        try
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id);

            if (sale == null || sale.UserId != userId)
            {
                return ApiResponse<bool>.ErrorResponse("Sale not found");
            }

            await _unitOfWork.Sales.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Sale deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete sale: {ex.Message}");
        }
    }

    private string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
