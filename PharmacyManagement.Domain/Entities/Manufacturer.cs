using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class Manufacturer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}
