using PharmacyManagement.Domain.Common;

namespace PharmacyManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PharmacyName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<CreditRecord> CreditRecords { get; set; } = new List<CreditRecord>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}