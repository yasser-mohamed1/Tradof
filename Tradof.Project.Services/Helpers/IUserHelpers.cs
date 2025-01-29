using Tradof.Data.Entities;

namespace Tradof.Project.Helpers
{
    public interface IUserHelpers
    {
        Task<ApplicationUser> GetCurrentUserAsync();
    }
}
