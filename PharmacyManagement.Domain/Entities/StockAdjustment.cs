using PharmacyManagement.Domain.Common;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Domain.Entities;

public class StockAdjustment : BaseEntity
{
    public string DrugId { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    public StockAdjustmentType Type { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    public int AdjustmentQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AdjustmentStatus Status { get; set; } = AdjustmentStatus.PENDING;
    public string RequestedBy { get; set; } = string.Empty;
    public string RequestedByName { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Drug Drug { get; set; } = null!;
    public Batch? Batch { get; set; }
    public User User { get; set; } = null!;
}
