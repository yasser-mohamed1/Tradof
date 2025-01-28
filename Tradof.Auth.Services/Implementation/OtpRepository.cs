using Microsoft.Extensions.Caching.Memory;
using Tradof.Auth.Services.Interfaces;

namespace Tradof.Auth.Services.Implementation
{
    public class OtpRepository(IMemoryCache cache) : IOtpRepository
    {
        private readonly IMemoryCache _cache = cache;

        public async Task SaveOtpAsync(string email, string otp, TimeSpan expiration)
        {
            _cache.Set(email, otp, expiration);
            await Task.CompletedTask;
        }

        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            if (_cache.TryGetValue(email, out string cachedOtp) && cachedOtp == otp)
            {
                _cache.Remove(email);
                return true;
            }
            return false;
        }
    }
}
