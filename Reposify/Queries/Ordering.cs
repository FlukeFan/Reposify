using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reposify.Queries
{
    public enum Direction
    {
        Ascending,
        Descending,
    }

    public class Ordering
    {
        public Expression   KeyExpression   { get; private set; }
        public Expression   KeyBody         { get; private set; }
        public Type         KeyType         { get; private set; }
        public Direction    Direction       { get; private set; }

        public static Ordering For<T, TKey>(Expression<Func<T, TKey>> property, Direction direction)
        {
            return new Ordering
            {
                KeyExpression = property,
                KeyBody = property.Body,
                KeyType = typeof(TKey),
                Direction = direction,
            };
        }

        public static Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> Lambda<T>(Ordering order)
        {
            var parameter = Expression.Parameter(typeof(IEnumerable<T>));
            var extensionType = typeof(Enumerable);
            var method = order.Direction == Direction.Ascending ? "OrderBy" : "OrderByDescending";
            var typeArguments = new Type[] { typeof(T), order.KeyType };
            var arguments = new Expression[] { parameter, order.KeyExpression };
            var call = Expression.Call(extensionType, method, typeArguments, arguments);
            var lambda = Expression.Lambda<Func<IEnumerable<T>, IOrderedEnumerable<T>>>(call, parameter);
            return lambda;
        }
    }
}
