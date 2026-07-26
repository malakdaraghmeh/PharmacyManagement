using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class SystemSettings : BaseEntity
{
    public string Currency { get; set; } = "USD";
    public decimal TaxPercentage { get; set; }
    public bool EnableLowStockNotification { get; set; } = true;
    public bool EnableExpiryNotification { get; set; } = true;
    public int ExpiryAlertDays { get; set; } = 30;
    public int MinimumPasswordLength { get; set; } = 6;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
}
