using Tradof.Data.Entities;

namespace Tradof.Auth.Services.Interfaces
{
    public interface IFreelancerRepository
    {
        Task AddAsync(Freelancer freelancer);
    }
}
