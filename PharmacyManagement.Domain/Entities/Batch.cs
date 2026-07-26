using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class Batch : BaseEntity
{
    public string DrugId { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public int RemainingQuantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string? SupplierId { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool IsExpired { get; set; }
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public Drug Drug { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public User User { get; set; } = null!;
}
