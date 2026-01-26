using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Infrastructure.Repositories;

public class SaleRepository : GenericRepository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Sale>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdWithItemsAsync(string id)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetSalesTodayAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(s => s.UserId == userId && s.CreatedAt.Date == today)
            .Include(s => s.SaleItems)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSalesTodayAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(s => s.UserId == userId && s.CreatedAt.Date == today)
            .SumAsync(s => s.NetAmount);
    }

    public async Task<int> GetInvoicesCountTodayAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .CountAsync(s => s.UserId == userId && s.CreatedAt.Date == today);
    }

    public async Task<IEnumerable<Sale>> GetRecentSalesAsync(string userId, int count = 10)
    {
        return await _dbSet
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}