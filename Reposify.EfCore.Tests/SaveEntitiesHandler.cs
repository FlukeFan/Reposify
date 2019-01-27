﻿using Reposify.EfCore;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class SaveEntitiesHandler : IEfCoreExecutionHandler<QuerySaveEntities>
    {
        // verify we can use the interface directly
        void IEfCoreExecutionHandler<QuerySaveEntities>.Execute(EfCoreRepository repository, QuerySaveEntities dbExecution)
        {
            foreach (var entity in dbExecution.EntitiesToSave)
                repository.Save(entity);
        }
    }
}