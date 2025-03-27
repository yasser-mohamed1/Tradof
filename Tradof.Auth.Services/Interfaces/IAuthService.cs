using Microsoft.AspNetCore.Http;
using Tradof.Auth.Services.DTOs;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterCompanyAsync(RegisterCompanyDto dto);
        Task RegisterFreelancerAsync(RegisterFreelancerDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<(string Token, string RefreshToken, string UserId, string Role)> LoginAsync(LoginDto dto);
        Task ForgetPasswordAsync(ForgetPasswordDto dto);
        Task VerifyOtpAsync(OtpVerificationDto dto);
        Task ChangePasswordWithTokenAsync(ChangePasswordDto dto);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<(string Id, string Role)> GetCurrentUserAsync();
        Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken);
        Task ResendOtpAsync(string email);
        Task<UserDto?> GetUserById(string id);
        Task<(string Token, string RefreshToken, string UserId, string Role)> AuthenticateWithGoogle(HttpContext httpContext);
    }
}