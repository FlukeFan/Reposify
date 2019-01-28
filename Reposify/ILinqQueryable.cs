using System.Linq;

namespace Reposify
{
    public interface ILinqQueryable
    {
        IQueryable<T> Query<T>() where T : class;
    }
}
