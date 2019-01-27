using System;
using NUnit.Framework;

namespace Reposify.Tests
{
    [TestFixture]
    public abstract class IDisposableTests
    {
        abstract protected IDisposable New();

        protected IDisposable _disposable;

        [SetUp]
        public virtual void SetUp()
        {
            _disposable = New();
        }

        [TearDown]
        public virtual void TearDown()
        {
            using (_disposable) { }
            _disposable = null;
        }
    }
}
