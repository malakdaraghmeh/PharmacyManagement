namespace PharmacyManagement.Application.DTOs.Stock;

public class ReceiveStockItemDto
{
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
}

public class ReceiveStockDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public string? Notes { get; set; }
    public List<ReceiveStockItemDto> Items { get; set; } = new();
}

public class ReceiveStockItemResponseDto
{
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class ReceiveStockResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public DateTime ReceivedAt { get; set; }
    public decimal TotalCost { get; set; }
    public string? Notes { get; set; }
    public List<ReceiveStockItemResponseDto> Items { get; set; } = new();
}
