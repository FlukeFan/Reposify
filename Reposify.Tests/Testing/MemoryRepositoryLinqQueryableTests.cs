using System;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New()
        {
            return new MemoryRepository(new ConstraintChecker());
        }
    }
}
