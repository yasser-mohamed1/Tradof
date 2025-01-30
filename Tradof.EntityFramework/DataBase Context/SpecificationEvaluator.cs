﻿using Microsoft.EntityFrameworkCore;
using Tradof.Data.Interfaces;

namespace Tradof.EntityFramework.DataBase_Context
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);
            if (spec.IsDistinctable)
                query = query.Distinct();
            if (spec.IsPaginable)
                query = query.Skip(spec.Skip).Take(spec.Take);

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
        public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> query, ISpecification<T, TResult> spec)
        {
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            var selectQuery = query as IQueryable<TResult>;
            if (spec.Select != null)
                selectQuery = query.Select(spec.Select);
            if (spec.IsDistinctable)
                selectQuery = selectQuery?.Distinct();
            if (spec.IsPaginable)
                selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
            return selectQuery ?? query.Cast<TResult>();
        }
    }
}
