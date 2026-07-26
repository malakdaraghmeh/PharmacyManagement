using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class StockReceiveItem : BaseEntity
{
    public string StockReceiveId { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Subtotal { get; set; }

    // Navigation properties
    public StockReceive StockReceive { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}
