using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Infrastructure.Repositories;

public class SaleItemRepository : GenericRepository<SaleItem>, ISaleItemRepository
{
    public SaleItemRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<SaleItem>> GetBySaleIdAsync(string saleId)
    {
        return await _dbSet
            .Where(si => si.SaleId == saleId)
            .ToListAsync();
    }
}