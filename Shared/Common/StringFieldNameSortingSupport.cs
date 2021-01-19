using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Common
{
    public static class StringFieldNameSortingSupport
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string fieldName)
            where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source,
            string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source,
            string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "ThenBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source,
            string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "ThenByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderUsingSortExpression<TEntity>(this IQueryable<TEntity> source,
            string sortExpression) where TEntity : class
        {
            var orderFields = sortExpression.Split(',');
            IOrderedQueryable<TEntity> result = null;
            for (var currentFieldIndex = 0; currentFieldIndex < orderFields.Length; currentFieldIndex++)
            {
                var expressionPart = orderFields[currentFieldIndex].Trim().Split(' ');
                var sortField = expressionPart[0];
                var sortDescending = expressionPart.Length == 2 &&
                                     expressionPart[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
                if (sortDescending)
                    result = currentFieldIndex == 0
                        ? source.OrderByDescending(sortField)
                        : result.ThenByDescending(sortField);
                else
                    result = currentFieldIndex == 0 ? source.OrderBy(sortField) : result.ThenBy(sortField);
            }

            return result;
        }

        #region Private expression tree helpers

        private static PropertyInfo GetProperty<TEntity>(string propertyName)
        {
            var properties =
                typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var property = properties.FirstOrDefault(m => string.Compare(m.Name, propertyName, true) == 0);

            return property;
        }

        private static LambdaExpression GenerateSelector<TEntity>(string propertyName, out Type resultType)
            where TEntity : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                //property = typeof(TEntity).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                property = GetProperty<TEntity>(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (var i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i],
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                // property = typeof(TEntity).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                property = GetProperty<TEntity>(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }

        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName,
            string fieldName) where TEntity : class
        {
            var type = typeof(TEntity);
            Type selectorResultType;
            var selector = GenerateSelector<TEntity>(fieldName, out selectorResultType);
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                new[] {type, selectorResultType},
                source.Expression, Expression.Quote(selector));
            return resultExp;
        }

        #endregion
    }
}