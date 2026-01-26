namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.CreditRecord;

public interface ICreditRecordService
{
    Task<ApiResponse<CreditRecordResponseDto>> CreateCreditRecordAsync(CreditRecordDto creditRecordDto, string userId);
    Task<ApiResponse<List<CreditRecordResponseDto>>> GetAllCreditRecordsAsync(string userId);
    Task<ApiResponse<CreditRecordResponseDto>> GetCreditRecordByIdAsync(string id, string userId);
    Task<ApiResponse<CreditRecordResponseDto>> UpdateCreditRecordAsync(string id, CreditRecordDto creditRecordDto, string userId);
    Task<ApiResponse<bool>> DeleteCreditRecordAsync(string id, string userId);
}