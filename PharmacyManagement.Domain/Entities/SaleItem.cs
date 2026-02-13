using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class SaleItem : BaseEntity
{
    public string SaleId { get; set; } = string.Empty;
    public string DrugId { get; set; } = string.Empty;
    public string DrugName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public Sale Sale { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}