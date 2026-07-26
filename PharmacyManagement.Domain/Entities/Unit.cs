using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
}
