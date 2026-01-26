using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Drug;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class DrugService : IDrugService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DrugService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DrugResponseDto>> CreateDrugAsync(DrugDto drugDto, string userId)
    {
        try
        {
            var drug = _mapper.Map<Drug>(drugDto);
            drug.UserId = userId;

            await _unitOfWork.Drugs.AddAsync(drug);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to create drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<DrugResponseDto>>> GetAllDrugsAsync(string userId)
    {
        try
        {
            var drugs = await _unitOfWork.Drugs.GetByUserIdAsync(userId);
            var response = _mapper.Map<List<DrugResponseDto>>(drugs);
            return ApiResponse<List<DrugResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<DrugResponseDto>>.ErrorResponse($"Failed to get drugs: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DrugResponseDto>> GetDrugByIdAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found");
            }

            var response = _mapper.Map<DrugResponseDto>(drug);
            return ApiResponse<DrugResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to get drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BarcodeDrugResponseDto>> GetDrugByBarcodeAsync(string barcode, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByBarcodeAsync(barcode);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<BarcodeDrugResponseDto>.ErrorResponse("Drug not found");
            }

            var response = _mapper.Map<BarcodeDrugResponseDto>(drug);
            return ApiResponse<BarcodeDrugResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<BarcodeDrugResponseDto>.ErrorResponse($"Failed to get drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DrugResponseDto>> UpdateDrugAsync(string id, DrugDto drugDto, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found");
            }

            _mapper.Map(drugDto, drug);
            await _unitOfWork.Drugs.UpdateAsync(drug);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to update drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DrugResponseDto>> ChangeDrugStatusAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found");
            }

            drug = await _unitOfWork.Drugs.ChangeStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug status changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to change drug status: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDrugAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<bool>.ErrorResponse("Drug not found");
            }

            await _unitOfWork.Drugs.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Drug deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete drug: {ex.Message}");
        }
    }
}