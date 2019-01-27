using System.Linq;

namespace Reposify.Tests
{
    public class QueryUsingLinq : IDbLinq<PolyType, int>
    {
        public string StringValue;

        public int Execute(IQueryable<PolyType> queryable)
        {
            return queryable
                .Where(q => q.String == StringValue)
                .Count();
        }
    }
}
