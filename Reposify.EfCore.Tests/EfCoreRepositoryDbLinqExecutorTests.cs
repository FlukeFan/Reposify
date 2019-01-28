using System;
using Reposify.Tests;

namespace Reposify.EfCore.Tests
{
    public class EfCoreRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New()
        {
            return EfCoreRepositoryTests.NewEfCoreRepository();
        }
    }
}
