using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Extensions
{
    public static class StringFieldNameSortingExtension
    {
        private static PropertyInfo GetProperty<TEntity>(string propertyName)
        {
            return typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                 BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                    string.Compare(m.Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static LambdaExpression GenerateSelector<TEntity>(
            string propertyName,
            out Type resultType)
            where TEntity : class
        {
            var parameterExpression = Expression.Parameter(typeof(TEntity), "Entity");
            PropertyInfo property;
            Expression expression;
            if (propertyName.Contains('.'))
            {
                var strArray = propertyName.Split('.');
                property = GetProperty<TEntity>(propertyName);
                expression =
                    Expression.MakeMemberAccess(parameterExpression, property);
                for (var index = 1; index < strArray.Length; ++index)
                {
                    property = property.PropertyType.GetProperty(strArray[index],
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    expression = Expression.MakeMemberAccess(expression, property!);
                }
            }
            else
            {
                property = GetProperty<TEntity>(propertyName);
                expression =
                    Expression.MakeMemberAccess(parameterExpression, property);
            }

            resultType = property.PropertyType;
            return Expression.Lambda(expression, parameterExpression);
        }

        private static MethodCallExpression GenerateMethodCall<TEntity>(
            IQueryable<TEntity> source,
            string methodName,
            string fieldName)
            where TEntity : class
        {
            var type = typeof(TEntity);
            Type resultType;
            var selector =
                GenerateSelector<TEntity>(fieldName, out resultType);
            return Expression.Call(typeof(Queryable), methodName, new Type[2]
            {
                type,
                resultType
            }, source.Expression, (Expression) Expression.Quote(selector));
        }

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
            this IQueryable<TEntity> source,
            string fieldName)
            where TEntity : class
        {
            var methodCall =
                GenerateMethodCall(source, nameof(OrderBy), fieldName);
            return source.Provider.CreateQuery<TEntity>(methodCall) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(
            this IQueryable<TEntity> source,
            string fieldName)
            where TEntity : class
        {
            var methodCall =
                GenerateMethodCall(source, nameof(OrderByDescending),
                    fieldName);
            return source.Provider.CreateQuery<TEntity>(methodCall) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(
            this IOrderedQueryable<TEntity> source,
            string fieldName)
            where TEntity : class
        {
            var methodCall =
                GenerateMethodCall(source,
                    nameof(ThenBy),
                    fieldName);
            return source.Provider.CreateQuery<TEntity>(methodCall) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(
            this IOrderedQueryable<TEntity> source,
            string fieldName)
            where TEntity : class
        {
            var methodCall =
                GenerateMethodCall(source,
                    nameof(ThenByDescending), fieldName);
            return source.Provider.CreateQuery<TEntity>(methodCall) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderUsingSortExpression<TEntity>(
            this IQueryable<TEntity> source,
            string sortExpression)
            where TEntity : class
        {
            var strArray1 = sortExpression.Split(',');
            IOrderedQueryable<TEntity> source1 = null;
            for (var index = 0; index < strArray1.Length; ++index)
            {
                var strArray2 = strArray1[index].Trim().Split(' ');
                var fieldName = strArray2[0];
                source1 = strArray2.Length != 2 || !strArray2[1].Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? index == 0 ? source.OrderBy(fieldName) : source1.ThenBy(fieldName)
                    : index == 0
                        ? source.OrderByDescending(fieldName)
                        : source1.ThenByDescending(fieldName);
            }

            return source1;
        }
    }
}