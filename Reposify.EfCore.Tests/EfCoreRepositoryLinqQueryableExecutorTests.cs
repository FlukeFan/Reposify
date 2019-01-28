using System;
using Reposify.Tests;

namespace Reposify.EfCore.Tests
{
    public class EfCoreRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New()
        {
            return EfCoreRepositoryTests.NewEfCoreRepository();
        }
    }
}
