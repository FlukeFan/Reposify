using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reposify.Queries
{
    public class WhereBinaryComparison : Where
    {
        public enum OperatorType
        {
            LessThan,
            LessThanOrEqual,
            Equal,
            NotEqual,
            GreaterThanOrEqual,
            GreaterThan,
        }

        public MemberInfo   Operand1 { get; protected set; }
        public OperatorType Operator { get; protected set; }
        public object       Operand2 { get; protected set; }

        public WhereBinaryComparison(Expression expression, MemberInfo operand1, ExpressionType expressionType, object operand2) : base(expression)
        {
            Operand1 = operand1;
            Operator = FindOperator(expressionType);
            Operand2 = operand2;
        }

        public override Expression CreateExpression(ParameterExpression parameter)
        {
            Expression left = Expression.PropertyOrField(parameter, Operand1.Name);
            var right = Expression.Constant(Operand2);
            var expressionType = FindExpressionType(Operator);

            if (left.Type != right.Type)
                left = Expression.Convert(left, right.Type);

            var comparison = Expression.MakeBinary(expressionType, left, right);
            return comparison;
        }

        public static OperatorType FindOperator(ExpressionType expressionType)
        {
            switch(expressionType)
            {
                case ExpressionType.LessThan:           return OperatorType.LessThan;
                case ExpressionType.LessThanOrEqual:    return OperatorType.LessThanOrEqual;
                case ExpressionType.Equal:              return OperatorType.Equal;
                case ExpressionType.NotEqual:           return OperatorType.NotEqual;
                case ExpressionType.GreaterThanOrEqual: return OperatorType.GreaterThanOrEqual;
                case ExpressionType.GreaterThan:        return OperatorType.GreaterThan;

                default:
                    throw new Exception("Unhandled binary comparison expression type: " + expressionType);
            }
        }

        public static ExpressionType FindExpressionType(OperatorType operatorType)
        {
            switch(operatorType)
            {
                case OperatorType.LessThan:             return ExpressionType.LessThan;
                case OperatorType.LessThanOrEqual:      return ExpressionType.LessThanOrEqual;
                case OperatorType.Equal:                return ExpressionType.Equal;
                case OperatorType.NotEqual:             return ExpressionType.NotEqual;
                case OperatorType.GreaterThanOrEqual:   return ExpressionType.GreaterThanOrEqual;
                case OperatorType.GreaterThan:          return ExpressionType.GreaterThan;

                default:
                    throw new Exception("Unhandled binary comparison operator type: " + operatorType);
            }
        }
    }
}
