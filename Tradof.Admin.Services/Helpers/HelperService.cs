using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Tradof.Data.Entities;

namespace Tradof.Admin.Services.Helpers
{
    public class HelperService(UserManager<ApplicationUser> _userManager) : IHelperService
    {
        #region GetAdmin
        public async Task<string> GetAdminAsync(ClaimsPrincipal user)
        {
            var userData = await _userManager.GetUserAsync(user);

            if (userData == null)
                return string.Empty;

            return userData.Id;
        }
        #endregion
    }
}
