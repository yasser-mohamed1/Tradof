namespace Tradof.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        IGeneralRepository<TEntity> Repository<TEntity>() where TEntity : class;
        void Dispose();

    }
}
