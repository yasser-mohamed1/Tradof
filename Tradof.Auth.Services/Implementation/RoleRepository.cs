using Microsoft.EntityFrameworkCore;
using Tradof.Auth.Services.Interfaces;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class RoleRepository(TradofDbContext _context) : IRoleRepository
    {

        public async Task<string> GetUserRoleAsync(string userId)
        {
            var role = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
            .Join(
                    _context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name
                )
                .FirstOrDefaultAsync();

            return role;
        }
    }
}
