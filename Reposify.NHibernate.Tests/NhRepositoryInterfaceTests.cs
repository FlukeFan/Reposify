using System;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class NhRepositoryAsyncTests : IRepositoryAsyncTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryDbExecutorTests : IDbExecutorTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryDbExecutorAsyncTests : IDbExecutorAsyncTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryDbLinqExecutorAsyncTests : IDbLinqExecutorAsyncTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }
}
