namespace Tradof.Auth.Services.Interfaces
{
    public interface IOtpRepository
    {
        Task SaveOtpAsync(string email, string otp, TimeSpan expiration);
        Task<bool> ValidateOtpAsync(string email, string otp);
    }
}
