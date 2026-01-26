namespace PharmacyManagement.Domain.Interfaces;

public interface ICreditRecordRepository : IGenericRepository<Entities.CreditRecord>
{
    Task<IEnumerable<Entities.CreditRecord>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Entities.CreditRecord>> GetOverdueCreditRecordsAsync(string userId);
}