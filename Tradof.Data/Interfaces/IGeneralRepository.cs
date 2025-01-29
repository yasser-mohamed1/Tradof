using System.Linq.Expressions;

namespace Tradof.Data.Interfaces
{
    public interface IGeneralRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(long id);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>> includes = null);

    }
}
