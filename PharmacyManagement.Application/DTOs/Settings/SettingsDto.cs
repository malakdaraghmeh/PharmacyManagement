namespace PharmacyManagement.Application.DTOs.Settings;

public class PharmacySettingsDto
{
    public string PharmacyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}

public class UpdatePharmacySettingsDto
{
    public string PharmacyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class SystemSettingsDto
{
    public string Currency { get; set; } = "USD";
    public decimal TaxPercentage { get; set; }
    public bool EnableLowStockNotification { get; set; }
    public bool EnableExpiryNotification { get; set; }
    public int ExpiryAlertDays { get; set; }
    public int MinimumPasswordLength { get; set; }
}
