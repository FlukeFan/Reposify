using System.Threading.Tasks;

namespace Reposify
{
    public interface IUnitOfWorkAsync
    {
        Task FlushAsync();
    }
}
