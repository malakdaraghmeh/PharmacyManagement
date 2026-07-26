namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Drug;

public interface IDrugService
{
    Task<ApiResponse<DrugResponseDto>> CreateDrugAsync(DrugDto drugDto, string userId);
    Task<PagedResponse<DrugListDto>> GetAllDrugsAsync(string userId, int page, int pageSize, string? name, string? barcode, string? categoryId, string? manufacturerId);
    Task<ApiResponse<DrugResponseDto>> GetDrugByIdAsync(string id, string userId);
    Task<ApiResponse<BarcodeDrugResponseDto>> GetDrugByBarcodeAsync(string barcode, string userId);
    Task<ApiResponse<DrugResponseDto>> UpdateDrugAsync(string id, DrugDto drugDto, string userId);
    Task<ApiResponse<DrugResponseDto>> ChangeDrugStatusAsync(string id, string userId);
    Task<ApiResponse<bool>> DeleteDrugAsync(string id, string userId);
    Task<ApiResponse<List<DrugListDto>>> GetLowStockDrugsAsync(string userId);
}