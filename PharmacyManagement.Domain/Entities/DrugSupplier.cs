using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

// Join entity for the many-to-many relationship between Drug and Supplier
public class DrugSupplier
{
    public string DrugId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;

    // Navigation properties
    public Drug Drug { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
}
