using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Infrastructure.Repositories;

public class CreditRecordRepository : GenericRepository<CreditRecord>, ICreditRecordRepository
{
    public CreditRecordRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<CreditRecord>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(cr => cr.UserId == userId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CreditRecord>> GetOverdueCreditRecordsAsync(string userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(cr => cr.UserId == userId &&
                        cr.DueDate < today &&
                        cr.Status != "Paid")
            .ToListAsync();
    }
}