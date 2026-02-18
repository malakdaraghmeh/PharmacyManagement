using PharmacyManagement.Domain.Common.Enums;
namespace PharmacyManagement.Application.DTOs.CreditRecord;

public class CreditRecordDto
{
    public TransactionType Type { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    // public decimal RemainingAmount { get; set; }
    public DateTime DueDate { get; set; }
    // public string Status { get; set; } = "Pending";
    public CreditStatus Status { get; set; } = CreditStatus.Pending;
    public string Notes { get; set; } = string.Empty;
}

public class CreditRecordResponseDto : CreditRecordDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal RemainingAmount { get; set; } 
}
