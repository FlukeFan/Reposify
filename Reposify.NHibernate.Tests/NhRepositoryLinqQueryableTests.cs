using System;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class NhRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New()
        {
            return NhRepositoryTests.NewNhRepository();
        }
    }
}
