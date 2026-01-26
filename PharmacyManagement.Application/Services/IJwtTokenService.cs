namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string? ValidateToken(string token);
}