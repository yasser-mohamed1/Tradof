using Microsoft.EntityFrameworkCore;
using Tradof.Auth.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class UserRepository(TradofDbContext _context) : IUserRepository
    {
        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTime expiryDate)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = expiryDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ApplicationUser> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}