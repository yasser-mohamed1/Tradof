using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Helpers
{
    public class UserHelpers(UserManager<ApplicationUser> _userManager
            , IHttpContextAccessor _contextAccessor) : IUserHelpers
    {

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var currentUser = _contextAccessor.HttpContext.User;
            return await _userManager.GetUserAsync(currentUser);
        }
    }
}
