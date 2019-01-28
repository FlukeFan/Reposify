using System;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class NhRepositoryDExecutorTests : IDbExecutorTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }

    public class NhRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New() { return NhRepositoryTests.NewNhRepository(); }
    }
}
