namespace PharmacyManagement.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<Entities.User>
{
    Task<Entities.User?> GetByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
}