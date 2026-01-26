using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class SaleItemService : ISaleItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SaleItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SaleItemResponseDto>> CreateSaleItemAsync(SaleItemDto saleItemDto, string userId)
    {
        try
        {
            var saleItem = _mapper.Map<SaleItem>(saleItemDto);
            await _unitOfWork.SaleItems.AddAsync(saleItem);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<SaleItemResponseDto>(saleItem);
            return ApiResponse<SaleItemResponseDto>.SuccessResponse(response, "Sale item created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SaleItemResponseDto>.ErrorResponse($"Failed to create sale item: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SaleItemResponseDto>>> GetAllSaleItemsAsync(string userId)
    {
        try
        {
            var saleItems = await _unitOfWork.SaleItems.GetAllAsync();
            var response = _mapper.Map<List<SaleItemResponseDto>>(saleItems);
            return ApiResponse<List<SaleItemResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SaleItemResponseDto>>.ErrorResponse($"Failed to get sale items: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SaleItemResponseDto>> GetSaleItemByIdAsync(string id, string userId)
    {
        try
        {
            var saleItem = await _unitOfWork.SaleItems.GetByIdAsync(id);

            if (saleItem == null)
            {
                return ApiResponse<SaleItemResponseDto>.ErrorResponse("Sale item not found");
            }

            var response = _mapper.Map<SaleItemResponseDto>(saleItem);
            return ApiResponse<SaleItemResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<SaleItemResponseDto>.ErrorResponse($"Failed to get sale item: {ex.Message}");
        }
    }

    public async Task<ApiResponse<SaleItemResponseDto>> UpdateSaleItemAsync(string id, SaleItemDto saleItemDto, string userId)
    {
        try
        {
            var saleItem = await _unitOfWork.SaleItems.GetByIdAsync(id);

            if (saleItem == null)
            {
                return ApiResponse<SaleItemResponseDto>.ErrorResponse("Sale item not found");
            }

            _mapper.Map(saleItemDto, saleItem);
            await _unitOfWork.SaleItems.UpdateAsync(saleItem);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<SaleItemResponseDto>(saleItem);
            return ApiResponse<SaleItemResponseDto>.SuccessResponse(response, "Sale item updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SaleItemResponseDto>.ErrorResponse($"Failed to update sale item: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSaleItemAsync(string id, string userId)
    {
        try
        {
            var saleItem = await _unitOfWork.SaleItems.GetByIdAsync(id);

            if (saleItem == null)
            {
                return ApiResponse<bool>.ErrorResponse("Sale item not found");
            }

            await _unitOfWork.SaleItems.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Sale item deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete sale item: {ex.Message}");
        }
    }
}