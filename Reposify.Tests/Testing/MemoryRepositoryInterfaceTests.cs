using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New() { return new MemoryRepository(new ConstraintChecker()); }
    }

    public class MemoryRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New() { return new MemoryRepository(new ConstraintChecker()); }
    }

    public class MemoryRepositoryDbExecutorTests : IDbExecutorTests
    {
        protected override IDisposable New() { return new MemoryRepository(new ConstraintChecker()); }

        private MemoryRepository Repository { get => (MemoryRepository)_disposable; }

        [Test]
        public override void DbQuery_IsImplemented()
        {
            Repository.SetHandler<QuerySaveEntities>(q =>
            {
                foreach (var e in q.EntitiesToSave)
                    Repository.Save(e);
            });

            Repository.SetHandler<QueryIn, IList<PolyType>>(q =>
                Repository.Query<PolyType>()
                    .Where(p => q.IntValues.Contains(p.Id))
                    .ToList());

            base.DbQuery_IsImplemented();
        }
    }
}
