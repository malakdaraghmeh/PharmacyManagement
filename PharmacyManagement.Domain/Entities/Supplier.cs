using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<DrugSupplier> DrugSuppliers { get; set; } = new List<DrugSupplier>();
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
