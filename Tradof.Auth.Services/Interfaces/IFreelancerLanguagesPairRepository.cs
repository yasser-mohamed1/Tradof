using Tradof.Data.Entities;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IFreelancerLanguagesPairRepository
    {
        Task AddRangeAsync(IEnumerable<FreelancerLanguagesPair> entities);
    }
}