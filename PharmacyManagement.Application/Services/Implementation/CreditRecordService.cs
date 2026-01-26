using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.CreditRecord;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class CreditRecordService : ICreditRecordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreditRecordService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CreditRecordResponseDto>> CreateCreditRecordAsync(CreditRecordDto creditRecordDto, string userId)
    {
        try
        {
            var creditRecord = _mapper.Map<CreditRecord>(creditRecordDto);
            creditRecord.UserId = userId;

            await _unitOfWork.CreditRecords.AddAsync(creditRecord);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CreditRecordResponseDto>(creditRecord);
            return ApiResponse<CreditRecordResponseDto>.SuccessResponse(response, "Credit record created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CreditRecordResponseDto>.ErrorResponse($"Failed to create credit record: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CreditRecordResponseDto>>> GetAllCreditRecordsAsync(string userId)
    {
        try
        {
            var creditRecords = await _unitOfWork.CreditRecords.GetByUserIdAsync(userId);
            var response = _mapper.Map<List<CreditRecordResponseDto>>(creditRecords);
            return ApiResponse<List<CreditRecordResponseDto>>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CreditRecordResponseDto>>.ErrorResponse($"Failed to get credit records: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CreditRecordResponseDto>> GetCreditRecordByIdAsync(string id, string userId)
    {
        try
        {
            var creditRecord = await _unitOfWork.CreditRecords.GetByIdAsync(id);

            if (creditRecord == null || creditRecord.UserId != userId)
            {
                return ApiResponse<CreditRecordResponseDto>.ErrorResponse("Credit record not found");
            }

            var response = _mapper.Map<CreditRecordResponseDto>(creditRecord);
            return ApiResponse<CreditRecordResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<CreditRecordResponseDto>.ErrorResponse($"Failed to get credit record: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CreditRecordResponseDto>> UpdateCreditRecordAsync(string id, CreditRecordDto creditRecordDto, string userId)
    {
        try
        {
            var creditRecord = await _unitOfWork.CreditRecords.GetByIdAsync(id);

            if (creditRecord == null || creditRecord.UserId != userId)
            {
                return ApiResponse<CreditRecordResponseDto>.ErrorResponse("Credit record not found");
            }

            _mapper.Map(creditRecordDto, creditRecord);
            await _unitOfWork.CreditRecords.UpdateAsync(creditRecord);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CreditRecordResponseDto>(creditRecord);
            return ApiResponse<CreditRecordResponseDto>.SuccessResponse(response, "Credit record updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CreditRecordResponseDto>.ErrorResponse($"Failed to update credit record: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCreditRecordAsync(string id, string userId)
    {
        try
        {
            var creditRecord = await _unitOfWork.CreditRecords.GetByIdAsync(id);

            if (creditRecord == null || creditRecord.UserId != userId)
            {
                return ApiResponse<bool>.ErrorResponse("Credit record not found");
            }

            await _unitOfWork.CreditRecords.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Credit record deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete credit record: {ex.Message}");
        }
    }
}
