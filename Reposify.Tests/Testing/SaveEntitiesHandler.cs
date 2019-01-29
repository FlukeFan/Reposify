using System.Threading.Tasks;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class SaveEntitiesHandler : IMemoryExecutionHandler<QuerySaveEntities>, IMemoryExecutionAsyncHandler<QuerySaveEntities>
    {
        // verify we can use the interface directly
        void IMemoryExecutionHandler<QuerySaveEntities>.Execute(MemoryRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                repository.Save(entity);
        }

        async Task IMemoryExecutionAsyncHandler<QuerySaveEntities>.ExecuteAsync(MemoryRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                await repository.SaveAsync(entity);
        }
    }
}
