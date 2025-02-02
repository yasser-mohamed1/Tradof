using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Helpers
{
    public interface IUserHelpers
    {
        Task<ApplicationUser> GetCurrentUserAsync();
    }
}
