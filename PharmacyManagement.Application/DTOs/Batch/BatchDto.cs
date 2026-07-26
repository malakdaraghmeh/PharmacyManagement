namespace PharmacyManagement.Application.DTOs.Batch;

public class BatchDto
{
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string? SupplierId { get; set; }
    public DateTime ReceivedAt { get; set; }
}

public class UpdateBatchDto : BatchDto
{
    public int RemainingQuantity { get; set; }
    public bool IsExpired { get; set; }
}

public class BatchResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string? SupplierId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public int RemainingQuantity { get; set; }
    public bool IsExpired { get; set; }
}
