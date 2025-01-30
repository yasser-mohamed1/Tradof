using System.Linq.Expressions;
using Tradof.Data.Interfaces;

namespace Tradof.Data.Specifications
{
    public class BaseSpecification<T>(Expression<Func<T, bool>>? _criteria) : ISpecification<T>
    {
        protected BaseSpecification() : this(null) { }
        public Expression<Func<T, bool>>? Criteria => _criteria;
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginable { get; private set; }
        public bool IsDistinctable { get; private set; }
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public List<Expression<Func<T, object>>> Includes { get; } = [];
        public List<string> IncludeStrings { get; } = [];

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void SetOrderBy(Expression<Func<T, object>> orderBy)
        {
            OrderBy = orderBy;
        }

        protected void SetOrderByDescending(Expression<Func<T, object>> orderByDescending)
        {
            OrderByDescending = orderByDescending;
        }

        public IQueryable<T> ApplyCriteria(IQueryable<T> query)
        {
            if (Criteria != null)
                query = query.Where(Criteria);
            return query;
        }

        protected void SetIsDistinctable() { IsDistinctable = true; }

        protected void ApplyPagination(int skip, int take)
        {
            Take = take;
            Skip = skip;
            IsPaginable = true;
        }
    }

    public class BaseSpecification<T, TResult>(Expression<Func<T, bool>> _criteria)
        : BaseSpecification<T>(_criteria), ISpecification<T, TResult>
    {
        protected BaseSpecification() : this(null!) { }
        public Expression<Func<T, TResult>>? Select { get; private set; }

        protected void SetSelect(Expression<Func<T, TResult>> select)
        {
            Select = select;
        }
    }
}
