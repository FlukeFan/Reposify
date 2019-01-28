using System;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class NhRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New()
        {
            return NhRepositoryTests.NewNhRepository();
        }
    }
}
