using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class QueryInAsyncHandler : IMemoryQueryAsyncHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public Task<IList<PolyType>> ExecuteAsync(MemoryRepository repository, QueryIn dbQuery)
        {
            return Task.FromResult<IList<PolyType>>(
                repository.All<PolyType>()
                    .Where(pt => dbQuery.IntValues.Contains(pt.Id))
                    .ToList());
        }
    }
}
