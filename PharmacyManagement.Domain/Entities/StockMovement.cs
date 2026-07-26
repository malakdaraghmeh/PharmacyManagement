using PharmacyManagement.Domain.Common;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Domain.Entities;

public class StockMovement : BaseEntity
{
    public string DrugId { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }
    public int RemainingAfter { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; } // sale, purchase, adjustment
    public string PerformedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Drug Drug { get; set; } = null!;
    public Batch? Batch { get; set; }
    public User User { get; set; } = null!;
}
