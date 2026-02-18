using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.CreditRecord;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Application.Services.Implementation;

public class CreditRecordService : ICreditRecordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
      public async Task<ApiResponse<CreditSummaryDto>> GetSummaryAsync(string userId)
    {
        try
        {
            var records = await _unitOfWork.CreditRecords.GetByUserIdAsync(userId);

            var summary = new CreditSummaryDto
            {
                TotalCredit = records.Where(x => x.Type == TransactionType.Credit).Sum(x => x.PaidAmount),
                TotalDebt = records.Where(x => x.Type == TransactionType.Debit).Sum(x => x.RemainingAmount),
                TotalPaid = records.Sum(x => x.PaidAmount)
            };

            return ApiResponse<CreditSummaryDto>.SuccessResponse(summary);
        }
        catch (Exception ex)
        {
            return ApiResponse<CreditSummaryDto>.ErrorResponse($"Failed to get summary: {ex.Message}");
        }
    }


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

            creditRecord.RemainingAmount = creditRecord.TotalAmount - creditRecord.PaidAmount;

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

   public async Task<ApiResponse<object>> GetAllCreditRecordsAsync(string userId)
{
    try
    {
        var creditRecords = await _unitOfWork.CreditRecords.GetByUserIdAsync(userId);

        var recordsResponse = _mapper.Map<List<CreditRecordResponseDto>>(creditRecords);

        var summary = new CreditSummaryDto
        {
            TotalCredit = creditRecords.Where(x => x.Type == TransactionType.Credit).Sum(x => x.TotalAmount),
            TotalDebt = creditRecords.Where(x => x.Type == TransactionType.Debit).Sum(x => x.TotalAmount),
            TotalPaid = creditRecords.Sum(x => x.PaidAmount)
        };

        // Return both
        var result = new
        {
            Records = recordsResponse,
            Summary = summary
        };

        return ApiResponse<object>.SuccessResponse(result, "Success");
    }
    catch (Exception ex)
    {
        return ApiResponse<object>.ErrorResponse($"Failed to get credit records: {ex.Message}");
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

            creditRecord.RemainingAmount = creditRecord.TotalAmount - creditRecord.PaidAmount;

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

