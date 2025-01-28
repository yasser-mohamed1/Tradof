using Tradof.Auth.Services.DTOs;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterCompanyAsync(RegisterCompanyDto dto);
        Task RegisterFreelancerAsync(RegisterFreelancerDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<(string Token, string UserId, string Role)> LoginAsync(LoginDto dto);
        Task ForgetPasswordAsync(ForgetPasswordDto dto);
        Task VerifyOtpAsync(OtpVerificationDto dto);
        Task ChangePasswordWithTokenAsync(ChangePasswordDto dto);
        Task<string> GeneratePasswordResetTokenAsync(string email);
    }
}