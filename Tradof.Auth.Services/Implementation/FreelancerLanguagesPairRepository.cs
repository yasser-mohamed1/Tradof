using Tradof.Auth.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class FreelancerLanguagesPairRepository(TradofDbContext _context) : IFreelancerLanguagesPairRepository
    {

        public async Task AddRangeAsync(IEnumerable<FreelancerLanguagesPair> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException("No language pairs provided to add.");
            }

            _context.FreelancerLanguagesPairs.AddRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}