using Reposify.Ef6;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class SaveEntitiesHandler : IEf6ExecutionHandler<QuerySaveEntities>
    {
        // verify we can use the interface directly
        void IEf6ExecutionHandler<QuerySaveEntities>.Execute(Ef6Repository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                repository.Save(entity);
        }
    }
}
