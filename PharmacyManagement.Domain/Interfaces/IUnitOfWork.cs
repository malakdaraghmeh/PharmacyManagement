namespace PharmacyManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IDrugRepository Drugs { get; }
    ISaleRepository Sales { get; }
    ISaleItemRepository SaleItems { get; }
    ICreditRecordRepository CreditRecords { get; }
    INotificationRepository Notifications { get; }

     DbContext DbContext { get; } 
 
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
