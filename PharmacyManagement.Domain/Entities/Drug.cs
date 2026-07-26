using PharmacyManagement.Domain.Common;
using PharmacyManagement.Domain.Common.Enums;

namespace PharmacyManagement.Domain.Entities;

public class Drug : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Packaging { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string? ManufacturerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MinimumStock { get; set; }
    public DrugStatus Status { get; set; } = DrugStatus.AVAILABLE;
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Manufacturer? Manufacturer { get; set; }
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<DrugSupplier> DrugSuppliers { get; set; } = new List<DrugSupplier>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}