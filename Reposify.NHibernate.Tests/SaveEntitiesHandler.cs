using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class SaveEntitiesHandler : INhExecutionHandler<int, QuerySaveEntities>
    {
        // verify we can use the interface directly
        void INhExecutionHandler<int, QuerySaveEntities>.Execute(NhRepository<int> repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                repository.Save(entity);
        }
    }
}
