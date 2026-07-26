using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class StockReceive : BaseEntity
{
    public string SupplierId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalCost { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Supplier Supplier { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<StockReceiveItem> Items { get; set; } = new List<StockReceiveItem>();
}
