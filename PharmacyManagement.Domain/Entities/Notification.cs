using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // LowStock, Expiring, Alert
    public bool IsRead { get; set; } = false;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
}