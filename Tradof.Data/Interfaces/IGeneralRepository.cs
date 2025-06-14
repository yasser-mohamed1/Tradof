﻿using System.Linq.Expressions;
using Tradof.Data.Entities;

namespace Tradof.Data.Interfaces
{
    public interface IGeneralRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, object>> orderBy = null, string direction = null, List<Expression<Func<T, object>>> includes = null);
        Task<T?> GetByIdAsync(long id, List<Expression<Func<T, object>>>? includes = null);
        Task<T?> GetByIdAsync(string id, List<Expression<Func<T, object>>>? includes = null);
        Task<T?> GetByUserIdAsync(string userId, List<Expression<Func<T, object>>>? includes = null);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(long id);
        Task<T> FindFirstAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>> includes = null);
        Task DeleteWithCrateriaAsync(Expression<Func<T, bool>> expression);
        Task<T?> GetEntityWithSpecification(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<TResult?> GetEntityWithSpecification<TResult>(ISpecification<T, TResult> spec);
        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec);
        //bool Exists(int id);
        Task<int> CountAsync(ISpecification<T> spec);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetListWithSpecificationAsync(ISpecification<T> spec);
        IQueryable<T> GetQueryable();
    }
}
