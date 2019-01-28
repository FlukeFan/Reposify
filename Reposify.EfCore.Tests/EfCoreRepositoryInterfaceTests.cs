using System;
using Reposify.Tests;

namespace Reposify.EfCore.Tests
{
    public class EfCoreRepositoryDbExecutorTests : IDbExecutorTests
    {
        protected override IDisposable New() { return EfCoreRepositoryTests.NewEfCoreRepository(); }
    }

    public class EfCoreRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New() { return EfCoreRepositoryTests.NewEfCoreRepository(); }
    }

    public class EfCoreRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New() { return EfCoreRepositoryTests.NewEfCoreRepository(); }
    }
}
