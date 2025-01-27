using Tradof.Auth.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Auth.Services.Implementation
{
    public class FreelancerLanguagesPairRepository : IFreelancerLanguagesPairRepository
    {
        private readonly TradofDbContext _context;

        public FreelancerLanguagesPairRepository(TradofDbContext context)
        {
            _context = context;
        }

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