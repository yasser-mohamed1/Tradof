using System.Collections.Concurrent;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.DataBase_Context;
using Tradof.Repository.Repository;

public class UnitOfWork(TradofDbContext _context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IGeneralRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;
        return (IGeneralRepository<TEntity>)_repositories.GetOrAdd(type, t =>
        {
            var repositoryType = typeof(GeneralRepository<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, _context)
                ?? throw new InvalidOperationException($"Could not create instance of {repositoryType}");
        });
    }
}
