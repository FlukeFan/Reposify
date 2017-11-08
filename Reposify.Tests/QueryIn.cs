using System.Collections.Generic;

namespace Reposify.Tests
{
    /// <summary> return the PolyType's with Int value contained in IntValues </summary>
    public class QueryIn : IDbQuery<IList<PolyType>>
    {
        public int[] IntValues;
    }
}
