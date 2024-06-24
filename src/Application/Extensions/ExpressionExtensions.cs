using System.Linq.Expressions;

namespace Template.Application.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<TEntity, bool>> And<TEntity>(this Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2)
        {
            return Combine(expr1, expr2, Expression.AndAlso);
        }

        public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2)
        {
            return Combine(expr1, expr2, Expression.OrElse);
        }

        public static Expression<Func<TEntity, bool>> Not<TEntity>(this Expression<Func<TEntity, bool>> expr)
        {
            var parameter = expr.Parameters.Single();
            var body = Expression.Not(expr.Body);
            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }

        public static Expression<Func<TEntity, bool>> AndNot<TEntity>(this Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2)
        {
            return expr1.And(expr2.Not());
        }

        private static Expression<Func<TEntity, bool>> Combine<TEntity>(
            Expression<Func<TEntity, bool>> expr1,
            Expression<Func<TEntity, bool>> expr2,
            Func<Expression, Expression, Expression> combineFunction)
        {
            var parameter = expr1.Parameters.Single();

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<TEntity, bool>>(combineFunction(left, right), parameter);
        }

        private sealed class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldExpr;
            private readonly Expression _newExpr;

            public ReplaceExpressionVisitor(Expression oldExpr, Expression newExpr)
            {
                _oldExpr = oldExpr;
                _newExpr = newExpr;
            }

            public override Expression Visit(Expression? node)
            {
                return node == _oldExpr ? _newExpr : base.Visit(node)!;
            }
        }
    }
}