using System.Collections.Concurrent;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Repository.Repositories
{
    public class UnitOfWork(TradofDbContext _context) : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new();

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose() => _context.Dispose();

        public IGeneralRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;
            return (IGeneralRepository<TEntity>)_repositories.GetOrAdd(type, t =>
            {
                var repositoryType = typeof(IGeneralRepository<>).MakeGenericType(typeof(TEntity));
                return Activator.CreateInstance(repositoryType, _context) ?? throw new InvalidOperationException($"could not create instance of {repositoryType}");
            });
        }
    }
}
