namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Auth;

public interface IProfileService
{
    Task<ApiResponse<RegisterResponseDto>> GetProfileAsync(string userId);
    Task<ApiResponse<RegisterResponseDto>> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto);
}

public class UpdateProfileDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PharmacyName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}