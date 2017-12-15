using System.Collections.Generic;
using NHibernate.Criterion;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class QueryInHandler : INhQueryHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public IList<PolyType> Execute(NhRepository repository, QueryIn dbquery)
        {
            return repository.Session.QueryOver<PolyType>()
                .Where(pt => pt.Id.IsIn(dbquery.IntValues))
                .List();
        }
    }
}
