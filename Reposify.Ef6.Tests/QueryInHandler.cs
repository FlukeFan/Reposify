using System.Collections.Generic;
using System.Linq;
using Reposify.Ef6;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class QueryInHandler : IEf6QueryHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public IList<PolyType> Execute(Ef6Repository repository, QueryIn dbquery)
        {
            return repository.DbContext.Set<PolyType>()
                .Where(pt => dbquery.IntValues.Contains(pt.Id))
                .ToList();
        }
    }
}
