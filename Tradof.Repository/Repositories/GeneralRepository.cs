using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Repository.Repository
{
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class
    {
        private readonly TradofDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(TradofDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(long id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }

        }
    }
}