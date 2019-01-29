using System.Linq;
using System.Threading.Tasks;

namespace Reposify.Tests
{
    public class QueryUsingLinq : IDbLinq<PolyType, int>, IDbLinqAsync<PolyType, int>
    {
        public string StringValue;

        public int Execute(IQueryable<PolyType> queryable)
        {
            return queryable
                .Where(q => q.String == StringValue)
                .Count();
        }

        public Task<int> ExecuteAsync(IQueryable<PolyType> queryable)
        {
            return Task.FromResult(queryable
                .Where(q => q.String == StringValue)
                .Count());
        }
    }
}
