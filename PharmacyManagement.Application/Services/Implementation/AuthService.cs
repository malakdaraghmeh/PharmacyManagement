using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Auth;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password");
            }

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password");
            }

            var token = _jwtTokenService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                PharmacyName = user.PharmacyName
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            if (await _unitOfWork.Users.UserExistsAsync(registerDto.Email))
            {
                return ApiResponse<RegisterResponseDto>.ErrorResponse("User with this email already exists");
            }

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<RegisterResponseDto>(user);
            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Registration successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<RegisterResponseDto>.ErrorResponse($"Registration failed: {ex.Message}");
        }
    }
}