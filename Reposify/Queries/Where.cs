using System;
using System.Linq.Expressions;

namespace Reposify.Queries
{
    public abstract class Where
    {
        public Expression Expression { get; private set; }

        public Where(Expression expression)
        {
            Expression = expression;
        }

        public static Where For<T>(Expression<Func<T, bool>> restriction)
        {
            return For(restriction.Body);
        }

        public static Expression<Func<T, bool>> Lambda<T>(Where where)
        {
            var parameter = Expression.Parameter(typeof(T));
            var expression = where.CreateExpression(parameter);
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            return lambda;
        }

        public abstract Expression CreateExpression(ParameterExpression parameter);

        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", base.ToString(), Expression.NodeType, Expression.GetType());
        }

        private static Where For(Expression restriction)
        {
            switch(ExpressionUtil.FindType(restriction))
            {
                case ExpressionUtil.ExpressionTypes.BinaryExpression:
                    return ForBinaryExpression((BinaryExpression)restriction);
                default:
                    throw ExpressionUtil.NewException("Unable to form query for: ", restriction);
            }
        }

        private static Where ForBinaryExpression(BinaryExpression binaryExpression)
        {
            var operand1 = ExpressionUtil.FindMemberInfo(binaryExpression.Left);
            var operand2 = ExpressionUtil.FindValue(binaryExpression.Right);
            return new WhereBinaryComparison(binaryExpression, operand1, binaryExpression.NodeType, operand2);
        }
    }
}
