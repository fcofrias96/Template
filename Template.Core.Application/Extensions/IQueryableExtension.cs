using System.Linq.Expressions;
using System.Reflection;

namespace Template.Core.Application.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> OrderByProperty<TEntity>(this IQueryable<TEntity> source, string propertyName, bool descending )
        {
            var entityType = typeof(TEntity);
            var property = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null)
            {
                throw new ArgumentException($"No se encontró la propiedad '{propertyName}' en la entidad '{entityType.FullName}'.");
            }

            var parameter = Expression.Parameter(entityType, "e");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            // Comprueba si se llamó al método "OrderBy" o "OrderByDescending"
            CustomExpressionVisitor visitor = new CustomExpressionVisitor("OrderBy");
            visitor.Visit(source.Expression);
            bool orderByCalled = visitor.MethodCalled;
            visitor = new CustomExpressionVisitor("OrderByDescending");
            visitor.Visit(source.Expression);
            bool orderByDescendingCalled = visitor.MethodCalled;

            string methodName;
            if (orderByCalled || orderByDescendingCalled)
            {
                methodName = descending ? "ThenByDescending" : "ThenBy";
            }
            else
            {
                methodName = descending ? "OrderByDescending" : "OrderBy";
            }

            var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { entityType, property.PropertyType },
                                                    source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public static IQueryable<TEntity> SearchInStringProperties<TEntity>(this IQueryable<TEntity> query, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            var entityType = typeof(TEntity);
            var parameter = Expression.Parameter(entityType, "e");
            Expression predicate = Expression.Constant(false);

            foreach (PropertyInfo property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(string)) continue;

                var propertyAccess = Expression.Property(parameter, property);
                var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsValue = Expression.Call(propertyAccess, containsMethod, Expression.Constant(searchTerm, typeof(string)));

                var stringPropertyCondition = Expression.AndAlso(nullCheck, containsValue);
                predicate = Expression.OrElse(predicate, stringPropertyCondition);
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);
            return query.Where(lambda);
        }
    }
    

    

    public class CustomExpressionVisitor : ExpressionVisitor
    {
        public bool MethodCalled { get; private set; }
        private readonly string _methodName;

        public CustomExpressionVisitor(string methodName)
        {
            _methodName = methodName;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == _methodName)
            {
                MethodCalled = true;
            }
            return base.VisitMethodCall(node);
        }
    }

}
