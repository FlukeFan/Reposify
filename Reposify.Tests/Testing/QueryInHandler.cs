using System.Collections.Generic;
using System.Linq;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class QueryInHandler : IMemoryQueryHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public IList<PolyType> Execute(MemoryRepository repository, QueryIn dbQuery)
        {
            return repository.All<PolyType>()
                .Where(pt => dbQuery.IntValues.Contains(pt.Id))
                .ToList();
        }
    }
}
