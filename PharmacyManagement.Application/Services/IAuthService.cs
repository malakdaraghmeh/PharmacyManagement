namespace PharmacyManagement.Application.Services;

using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Auth;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto);
}