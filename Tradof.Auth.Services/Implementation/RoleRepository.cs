using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Auth.Services.Interfaces;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TradofDbContext _context;

        public RoleRepository(TradofDbContext context)
        {
            _context = context;
        }

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
