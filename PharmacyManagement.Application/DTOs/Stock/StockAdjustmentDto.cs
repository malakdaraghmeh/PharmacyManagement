using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Application.DTOs.Stock;

public class StockAdjustmentDto
{
    public string DrugId { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    public StockAdjustmentType Type { get; set; }
    public int AdjustmentQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class StockAdjustmentResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public StockAdjustmentType Type { get; set; }
    public int QuantityBefore { get; set; }
    public int QuantityAfter { get; set; }
    public int AdjustmentQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AdjustmentStatus Status { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string RequestedByName { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
