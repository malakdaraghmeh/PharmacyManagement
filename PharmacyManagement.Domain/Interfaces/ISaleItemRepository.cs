namespace PharmacyManagement.Domain.Interfaces;

public interface ISaleItemRepository : IGenericRepository<Entities.SaleItem>
{
    Task<IEnumerable<Entities.SaleItem>> GetBySaleIdAsync(string saleId);
}