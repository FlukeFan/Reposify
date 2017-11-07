using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reposify.Queries
{
    public static class ExpressionUtil
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
                case ExpressionTypes.UnaryExpression:
                    return FindMemberInfo(((UnaryExpression)expression).Operand);
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
                return  ExpressionTypes.UnaryExpression;
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

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            // https://stackoverflow.com/a/16043551/357728
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }
    }
}
