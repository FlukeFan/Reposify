using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class QueryInHandler : IMemoryQueryHandler<QueryIn, IList<PolyType>>, IMemoryQueryAsyncHandler<QueryIn, IList<PolyType>>
    {
        // implement interface implicitly
        public IList<PolyType> Execute(MemoryRepository repository, QueryIn dbQuery)
        {
            return repository.All<PolyType>()
                .Where(pt => dbQuery.IntValues.Contains(pt.Id))
                .ToList();
        }

        public Task<IList<PolyType>> ExecuteAsync(MemoryRepository repository, QueryIn dbQuery)
        {
            return Task.FromResult<IList<PolyType>>(
                repository.All<PolyType>()
                    .Where(pt => dbQuery.IntValues.Contains(pt.Id))
                    .ToList());
        }
    }
}
