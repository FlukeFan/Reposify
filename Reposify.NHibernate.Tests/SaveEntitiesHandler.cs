using System.Threading.Tasks;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class SaveEntitiesHandler : INhExecutionHandler<QuerySaveEntities>, INhExecutionAsyncHandler<QuerySaveEntities>
    {
        // verify we can use the interface directly
        void INhExecutionHandler<QuerySaveEntities>.Execute(NhRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                repository.Save(entity);
        }

        async Task INhExecutionAsyncHandler<QuerySaveEntities>.ExecuteAsync(NhRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                await repository.SaveAsync(entity);
        }
    }
}
