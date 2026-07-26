using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Infrastructure.Repositories;

public class DrugRepository : GenericRepository<Drug>, IDrugRepository
{
    public DrugRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Drug?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Barcode == barcode);
    }

    public async Task<IEnumerable<Drug>> GetByUserIdAsync(string userId)
    {
        return await _dbSet.Where(d => d.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Drug>> GetLowStockDrugsAsync(string userId)
    {
        var drugs = await _dbSet.Where(d => d.UserId == userId).ToListAsync();

        var stockByDrug = await _context.Set<Batch>()
            .Where(b => b.UserId == userId)
            .GroupBy(b => b.DrugId)
            .Select(g => new { DrugId = g.Key, Total = g.Sum(x => x.RemainingQuantity) })
            .ToDictionaryAsync(x => x.DrugId, x => x.Total);

        return drugs.Where(d => (stockByDrug.TryGetValue(d.Id, out var total) ? total : 0) <= d.MinimumStock);
    }

    public async Task<IEnumerable<Drug>> GetExpiringDrugsAsync(string userId, int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        var expiringDrugIds = await _context.Set<Batch>()
            .Where(b => b.UserId == userId && b.RemainingQuantity > 0 && b.ExpiryDate <= thresholdDate)
            .Select(b => b.DrugId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Where(d => d.UserId == userId && expiringDrugIds.Contains(d.Id))
            .ToListAsync();
    }

    public async Task<Drug> ChangeStatusAsync(string id)
    {
        var drug = await GetByIdAsync(id);
        if (drug != null)
        {
            drug.IsActive = !drug.IsActive;
            drug.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(drug);
        }
        return drug!;
    }
}