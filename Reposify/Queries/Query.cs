using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reposify.Queries
{
    public class Query<T, TId> where T : IEntity<TId>
    {
        private IRepository<TId>    _repository;
        private IList<Where>        _restrictions   = new List<Where>();
        private IList<Ordering>     _orders         = new List<Ordering>();

        public IEnumerable<Where>       Restrictions    { get { return _restrictions; } }
        public IEnumerable<Ordering>    Orders          { get { return _orders; } }

        public int?                     SkipCount       { get; protected set; }
        public int?                     TakeCount       { get; protected set; }

        public Query(IRepository<TId> repository)
        {
            _repository = repository;
        }

        public Query<T, TId> Filter(Expression<Func<T, bool>> restriction)
        {
            _restrictions.Add(Where.For(restriction));
            return this;
        }

        public Query<T, TId> OrderBy<TKey>(Expression<Func<T, TKey>> property)
        {
            _orders.Add(Ordering.For(property, Direction.Ascending));
            return this;
        }

        public Query<T, TId> OrderByDescending<TKey>(Expression<Func<T, TKey>> property)
        {
            _orders.Add(Ordering.For(property, Direction.Descending));
            return this;
        }

        public Query<T, TId> Skip(int skipCount)
        {
            SkipCount = skipCount;
            return this;
        }

        public Query<T, TId> Take(int takeCount)
        {
            TakeCount = takeCount;
            return this;
        }


        public IList<T> List()
        {
            return _repository.Satisfy(this);
        }

        public T SingleOrDefault()
        {
            return List().SingleOrDefault();
        }
    }
}
