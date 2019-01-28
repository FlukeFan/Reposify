using System;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New()
        {
            return new MemoryRepository(new ConstraintChecker());
        }
    }
}
