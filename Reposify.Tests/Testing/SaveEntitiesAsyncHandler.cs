using System.Threading.Tasks;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class SaveEntitiesAsyncHandler : IMemoryExecutionAsyncHandler<QuerySaveEntities>
    {
        // verify we can use the interface directly
        async Task IMemoryExecutionAsyncHandler<QuerySaveEntities>.ExecuteAsync(MemoryRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                await repository.SaveAsync(entity);
        }
    }
}
