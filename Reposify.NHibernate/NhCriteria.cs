using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Reposify.Queries;

namespace Reposify.NHibernate
{
    public static class NhCriteria
    {
        public static NhCriteria<T, TId> For<T, TId>(Query<T, TId> query) where T : IEntity<TId>
        {
            return new NhCriteria<T, TId>(query);
        }
    }

    public class NhCriteria<T, TId> where T : IEntity<TId>
    {
        private static IDictionary<Type, Action<ICriteria, Where>> _restrictionProcessors = new Dictionary<Type, Action<ICriteria, Where>>
        {
            { typeof(WhereBinaryComparison), (criteria, where) => AddBinaryComparison(criteria, (WhereBinaryComparison)where) },
        };

        private static IDictionary<WhereBinaryComparison.OperatorType, Func<string, object, ICriterion>> _binaryComparisons = new Dictionary<WhereBinaryComparison.OperatorType, Func<string, object, ICriterion>>
        {
            { WhereBinaryComparison.OperatorType.LessThan,              (prop, val) => Restrictions.Lt(prop, val) },
            { WhereBinaryComparison.OperatorType.LessThanOrEqual,       (prop, val) => Restrictions.Le(prop, val) },
            { WhereBinaryComparison.OperatorType.Equal,                 (prop, val) => Restrictions.Eq(prop, val) },
            { WhereBinaryComparison.OperatorType.GreaterThanOrEqual,    (prop, val) => Restrictions.Ge(prop, val) },
            { WhereBinaryComparison.OperatorType.GreaterThan,           (prop, val) => Restrictions.Gt(prop, val) },
        };

        private static IDictionary<Direction, Func<string, Order>> _orders = new Dictionary<Direction, Func<string, Order>>
        {
            { Direction.Ascending,  (prop) => Order.Asc(prop)   },
            { Direction.Descending, (prop) => Order.Desc(prop)  },
        };

        public Query<T, TId> Query { get; protected set; }

        public NhCriteria(Query<T, TId> query)
        {
            Query = query;
        }

        public ICriteria CreateCriteria(ISession session)
        {
            var criteria = session.CreateCriteria(typeof(T));

            foreach (var restriction in Query.Restrictions)
                AddRestriction(criteria, restriction);

            foreach (var order in Query.Orders)
                AddOrder(criteria, order);

            if (Query.SkipCount.HasValue)
                criteria.SetFirstResult(Query.SkipCount.Value);

            if (Query.TakeCount.HasValue)
                criteria.SetMaxResults(Query.TakeCount.Value);

            return criteria;
        }

        private void AddRestriction(ICriteria criteria, Where where)
        {
            var whereType = where.GetType();

            if (!_restrictionProcessors.ContainsKey(whereType))
                throw new Exception("Unhandled Where clause: " + where);

            var processor = _restrictionProcessors[whereType];
            processor(criteria, where);
        }

        private static void AddBinaryComparison(ICriteria criteria, WhereBinaryComparison where)
        {
            if (!_binaryComparisons.ContainsKey(where.Operator))
                throw new Exception("Unhandled comparison operator: " + where.Operator);

            var criterionFunc = _binaryComparisons[where.Operator];
            var criterion = criterionFunc(where.Operand1.Name, where.Operand2);
            criteria.Add(criterion);
        }

        private void AddOrder(ICriteria criteria, Ordering ordering)
        {
            var orderFunc = _orders[ordering.Direction];
            var member = ExpressionUtil.FindMemberInfo(ordering.KeyBody);
            var order = orderFunc(member.Name);
            criteria.AddOrder(order);
        }
    }
}
