using Tradof.Data.Entities;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime expiryDate);
        Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken);
    }
}