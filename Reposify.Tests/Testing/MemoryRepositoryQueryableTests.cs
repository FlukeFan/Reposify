using System;
using Reposify.Testing;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryQueryableTests : IQueryableRepositoryTests
    {
        protected override IDisposable New()
        {
            return new MemoryRepository(new ConstraintChecker());
        }
    }
}
