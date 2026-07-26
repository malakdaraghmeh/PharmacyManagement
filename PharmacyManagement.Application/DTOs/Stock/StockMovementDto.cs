using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Application.DTOs.Stock;

public class StockMovementResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string? BatchId { get; set; }
    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }
    public int RemainingAfter { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
