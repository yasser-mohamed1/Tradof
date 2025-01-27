using Tradof.Data.Entities;

namespace Tradof.Auth.Services.Interfaces
{
    public interface ICompanyRepository
    {
        Task AddAsync(Company company);
    }
}
