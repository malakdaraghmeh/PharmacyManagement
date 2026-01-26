using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class CreditRecord : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue
    public string Notes { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
}