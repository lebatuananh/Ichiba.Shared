using System.Linq;
using Shared.Common.Sort;

namespace Shared.Extensions
{
    public static class SortExtensions
    {
        public static IQueryable<TEntity> ToSort<TEntity>(this IQueryable<TEntity> query, Sorts sorts)
            where TEntity : class
        {
            var queryable = query;
            if (sorts != null && sorts.Any() && !string.IsNullOrWhiteSpace(sorts.SortExpression))
                queryable = query.OrderUsingSortExpression(sorts.SortExpression);
            return queryable;
        }
    }
}