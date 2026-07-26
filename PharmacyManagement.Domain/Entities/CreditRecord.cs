
﻿using PharmacyManagement.Domain.Common;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Domain.Entities;

public class CreditRecord : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; } // EF Core can map this
    public DateTime DueDate { get; set; }
    // public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue
    public CreditStatus Status { get; set; } = CreditStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public TransactionType Type { get; set; } = TransactionType.Credit;
    public string? RelatedSaleId { get; set; }
    // Navigation properties
    public User User { get; set; } = null!;
}
