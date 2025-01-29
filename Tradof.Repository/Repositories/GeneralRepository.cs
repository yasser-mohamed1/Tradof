using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Repository.Repository
{
    public class GeneralRepository<T>(TradofDbContext _context) : IGeneralRepository<T> where T : class
    {

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(long id) => await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().Where(predicate).ToListAsync();
        public async Task<T> FindFirstAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>> includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(expression);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }

        }
    }
}