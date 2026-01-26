namespace PharmacyManagement.Domain.Interfaces;

public interface IDrugRepository : IGenericRepository<Entities.Drug>
{
    Task<Entities.Drug?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<Entities.Drug>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Entities.Drug>> GetLowStockDrugsAsync(string userId);
    Task<IEnumerable<Entities.Drug>> GetExpiringDrugsAsync(string userId, int daysThreshold = 30);
    Task<Entities.Drug> ChangeStatusAsync(string id);
}