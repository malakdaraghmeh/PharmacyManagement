using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public SaleService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<SaleResponseDto>> CreateSaleAsync(CreateSaleDto saleDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Create sale
            var sale = _mapper.Map<Sale>(saleDto);
            sale.UserId = userId;
            sale.InvoiceNumber = GenerateInvoiceNumber();

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            // Create sale items and update drug quantities
            foreach (var itemDto in saleDto.Items)
            {
                var drug = await _unitOfWork.Drugs.GetByIdAsync(itemDto.DrugId);
                if (drug == null || drug.UserId != userId)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<SaleResponseDto>.ErrorResponse($"Drug {itemDto.DrugName} not found");
                }

                if (drug.Quantity < itemDto.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<SaleResponseDto>.ErrorResponse($"Insufficient quantity for {itemDto.DrugName}");
                }

                // Update drug quantity
                drug.Quantity -= itemDto.Quantity;
                await _unitOfWork.Drugs.UpdateAsync(drug);

                // Create sale item
                var saleItem = _mapper.Map<SaleItem>(itemDto);
                saleItem.SaleId = sale.Id;
                await _unitOfWork.SaleItems.AddAsync(saleItem);

                // Check if stock is low
                if (drug.Quantity <= drug.MinimumStock)
                {
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        "Low Stock Alert",
                        $"{drug.Name} is running low. Current quantity: {drug.Quantity}",
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
