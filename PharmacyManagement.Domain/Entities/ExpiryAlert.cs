using PharmacyManagement.Domain.Common;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Domain.Entities;

public class ExpiryAlert : BaseEntity
{
    public string BatchId { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int RemainingQuantity { get; set; }
    public int DaysToExpire { get; set; }
    public ExpiryStatus Status { get; set; }
    public ExpirySeverity Severity { get; set; }
    public decimal EstimatedLossValue { get; set; }
    public ExpiryAction RecommendedAction { get; set; }
    public bool IsAcknowledged { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Batch Batch { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public User User { get; set; } = null!;
}
