using Tradof.Data.Entities;

namespace Tradof.Proposal.Services.Helpers
{
    public interface IUserHelpers
    {
        Task<ApplicationUser> GetCurrentUserAsync();
    }
}
