namespace PharmacyManagement.Domain.Interfaces;

public interface INotificationRepository : IGenericRepository<Entities.Notification>
{
    Task<IEnumerable<Entities.Notification>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Entities.Notification>> GetUnreadByUserIdAsync(string userId);
    Task<Entities.Notification> MarkAsReadAsync(string id);
}