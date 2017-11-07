using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reposify.Queries
{
    public class ExpressionUtil
    {
        public enum ExpressionTypes
        {
            Unrecognised,
            BinaryExpression,
            MemberExpression,
            UnaryExpression,
        };

        public static MemberInfo FindMemberInfo(Expression expression)
        {
            switch (FindType(expression))
            {
                case ExpressionTypes.MemberExpression:
                    return FindMemberInfo((MemberExpression)expression);
                default:
                    throw NewException("Expected property access (like e.Name).  Unabled to find MemberInfo for: ", expression);
            }
        }

        public static MemberInfo FindMemberInfo(MemberExpression memberExpression)
        {
            return memberExpression.Member;
        }

        public static ExpressionTypes FindType(Expression expression)
        {
            if (expression is BinaryExpression)
                return ExpressionTypes.BinaryExpression;
            else if (expression is MemberExpression)
                return ExpressionTypes.MemberExpression;
            else if (expression is UnaryExpression)
                return ExpressionTypes.UnaryExpression;
            else
                return ExpressionTypes.Unrecognised;
        }

        public static object FindValue(Expression expression)
        {
            var valueExpression = Expression.Lambda(expression).Compile();
            object value = valueExpression.DynamicInvoke();
            return value;
        }

        public static Exception NewException(string message, Expression expression)
        {
            return new Exception(string.Format("{0} {1} of type ({2}, {3})", message, expression, expression.NodeType, expression.GetType()));
        }
    }
}
