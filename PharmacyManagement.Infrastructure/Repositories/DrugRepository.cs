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
        return await _dbSet
            .Where(d => d.UserId == userId && d.Quantity <= d.MinimumStock)
            .ToListAsync();
    }

    public async Task<IEnumerable<Drug>> GetExpiringDrugsAsync(string userId, int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await _dbSet
            .Where(d => d.UserId == userId && d.ExpiryDate <= thresholdDate)
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