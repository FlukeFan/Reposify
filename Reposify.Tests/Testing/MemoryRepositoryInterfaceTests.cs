using System;

namespace Reposify.Tests.Testing
{
    public class MemoryRepositoryAsyncTests : IRepositoryAsyncTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }

    public class MemoryRepositoryDbLinqExecutorTests : IDbLinqExecutorTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }

    public class MemoryRepositoryDbLinqExecutorAsyncTests : IDbLinqExecutorAsyncTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }

    public class MemoryRepositoryLinqQueryableTests : ILinqQueryableTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }

    public class MemoryRepositoryDbExecutorTests : IDbExecutorTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }

    public class MemoryRepositoryDbExecutorAsyncTests : IDbExecutorAsyncTests
    {
        protected override IDisposable New() { return MemoryRepositoryTests.NewMemoryRepository(); }
    }
}
