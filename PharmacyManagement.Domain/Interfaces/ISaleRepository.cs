namespace PharmacyManagement.Domain.Interfaces;

public interface ISaleRepository : IGenericRepository<Entities.Sale>
{
    Task<IEnumerable<Entities.Sale>> GetByUserIdAsync(string userId);
    Task<Entities.Sale?> GetByIdWithItemsAsync(string id);
    Task<IEnumerable<Entities.Sale>> GetSalesTodayAsync(string userId);
    Task<decimal> GetTotalSalesTodayAsync(string userId);
    Task<int> GetInvoicesCountTodayAsync(string userId);
    Task<IEnumerable<Entities.Sale>> GetRecentSalesAsync(string userId, int count = 10);
}