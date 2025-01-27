using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IRoleRepository
    {
        Task<string> GetUserRoleAsync(string userId);
    }
}
