using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Tests
{
    public abstract class IRepositoryAsyncTests : IDisposableTests
    {
        private IRepositoryAsync    Repository  { get => (IRepositoryAsync)_disposable; }

        [Test]
        public async virtual Task Save_SetsId()
        {
            var poly = new PolyTypeBuilder().Value();

            poly.Id.Should().Be(0, "newly instantiated entity should have a default Id value");

            var savedUser = await Repository.SaveAsync(poly);

            savedUser.Should().BeSameAs(poly);
            savedUser.Id.Should().NotBe(0, "persisted entity should have a non-zero Id");
        }

        [Test]
        public async virtual Task Load_RetrievesSavedObject()
        {
            var poly = new PolyTypeBuilder().Value();

            await Repository.SaveAsync(poly);
            poly.Id.Should().NotBe(0);

            var loaded = await Repository.LoadAsync<PolyType>(poly.Id);
            loaded.Should().BeSameAs(poly);
        }

        [Test]
        public async virtual Task Delete_RemovesEntity()
        {
            var poly = await new PolyTypeBuilder().SaveAsync(Repository);

            await Repository.DeleteAsync(poly);

            Assert.That(async () =>
            {
                var saved = await Repository.LoadAsync<PolyType>(poly.Id);
                saved.String.Should().Be(poly.String);
            }, Throws.Exception);
        }
    }
}
