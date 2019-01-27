using System.Collections.Generic;
using System.Linq;
using Reposify.EfCore;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class QueryInHandler : IEfCoreQueryHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public IList<PolyType> Execute(EfCoreRepository repository, QueryIn dbquery)
        {
            return repository.DbContext.Set<PolyType>()
                .Where(pt => dbquery.IntValues.Contains(pt.Id))
                .ToList();
        }
    }
}
