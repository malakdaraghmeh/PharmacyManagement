using AutoMapper;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Auth;
using PharmacyManagement.Domain.Interfaces;

namespace PharmacyManagement.Application.Services.Implementation;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProfileService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RegisterResponseDto>> GetProfileAsync(string userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<RegisterResponseDto>.ErrorResponse("User not found");
            }

            var response = _mapper.Map<RegisterResponseDto>(user);
            return ApiResponse<RegisterResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<RegisterResponseDto>.ErrorResponse($"Failed to get profile: {ex.Message}");
        }
    }

    public async Task<ApiResponse<RegisterResponseDto>> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<RegisterResponseDto>.ErrorResponse("User not found");
            }

            _mapper.Map(updateProfileDto, user);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<RegisterResponseDto>(user);
            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<RegisterResponseDto>.ErrorResponse($"Failed to update profile: {ex.Message}");
        }
    }
}