// File: Domain/Interfaces/ISaleRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PharmacyManagement.Domain.Entities; // for Sale

namespace PharmacyManagement.Domain.Interfaces
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetByUserIdAsync(string userId);
        Task<Sale?> GetByIdWithItemsAsync(string id);
        Task<IEnumerable<Sale>> GetSalesTodayAsync(string userId);
        Task<decimal> GetTotalSalesTodayAsync(string userId);
        Task<int> GetInvoicesCountTodayAsync(string userId);
        Task<IEnumerable<Sale>> GetRecentSalesAsync(string userId, int count = 10);

        IQueryable<Sale> GetAllSalesWithItemsAsyncQueryable();
        Task<List<Sale>> GetAllSalesWithItemsAsync();
    }
}