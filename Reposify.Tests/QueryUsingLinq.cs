using System.Linq;

namespace Reposify.Tests
{
    public class QueryUsingLinq : IDbLinq<PolyType>
    {
        public string StringValue;

        public IQueryable<PolyType> Prepare(IQueryable<PolyType> queryable)
        {
            return queryable
                .Where(q => q.String == StringValue);
        }
    }
}
