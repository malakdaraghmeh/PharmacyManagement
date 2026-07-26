using PharmacyManagement.Domain.Common.Enums;
namespace PharmacyManagement.Application.DTOs.CreditRecord;

public class CreditRecordDto
{
    public TransactionType Type { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? RelatedSaleId { get; set; }
}

public class CreditRecordResponseDto : CreditRecordDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreditRecordsResponseDto
{
    public List<CreditRecordResponseDto> Records { get; set; } = new();
    public CreditSummaryDto Summary { get; set; } = new();
}
