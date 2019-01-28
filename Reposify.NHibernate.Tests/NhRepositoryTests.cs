using System;
using FluentAssertions;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using Reposify.Database.Tests;
using Reposify.Testing;
using Reposify.Tests;

namespace Reposify.NHibernate.Tests
{
    public class NhRepositoryTests : IRepositoryTests
    {
        private static ISessionFactory  _sessionFactory;
        private static NhHandlers       _handlers;

        static NhRepositoryTests()
        {
            var environment = BuildEnvironment.Load();

            var mapper = new ConventionModelMapper();
            mapper.BeforeMapProperty += (modelInspector, member, propertyCustomizer) =>
            {
                if (member.LocalMember.Name == "BigString")
                    propertyCustomizer.Type(NHibernateUtil.StringClob);
            };

            var mappings = NhHelper.CreateConventionalMappings<TestsEntity>(mapper);
            var config = NhHelper.CreateConfig(mappings, cfg =>
            {
                cfg.DataBaseIntegration(db =>
                {
                    db.ConnectionString = environment.Connection;
                    db.ConnectionProvider<DriverConnectionProvider>();
                    db.Driver<SqlClientDriver>();
                    db.Dialect<MsSql2008Dialect>();
                });
            });

            _sessionFactory = config.BuildSessionFactory();
            _handlers = new NhHandlers().UsingHandlersFromAssemblyForType<NhRepositoryTests>();
        }

        public static NhRepository NewNhRepository()
        {
            return NhRepository.Open(_sessionFactory, _handlers);
        }

        protected override IDisposable New()
        {
            return NewNhRepository();
        }

        private NhRepository Repository { get => (NhRepository)_disposable; }

        [Test]
        public void Flush_UpdatesUnderlyingTable()
        {
            var poly = new PolyTypeBuilder()
                .With(p => p.String, "initial value")
                .Value();

            Repository.Save(poly);
            Repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value");

            Builder.Modify(poly).With(p => p.String, "modified value");

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value", "DB has not been updated");

            Repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("modified value", "in-memory changes have been flushed to the DB");
        }

        [Test]
        public void Clear_PurgesIdentityMap()
        {
            var poly = new PolyTypeBuilder()
                .With(p => p.String, "initial value")
                .Value();

            Repository.Save(poly);
            Repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value");

            Builder.Modify(poly).With(p => p.String, "modified value");

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value", "DB has not been updated");

            Repository.Clear();

            var loaded = Repository.Load<PolyType>(poly.Id);

            loaded.String.Should().Be("initial value");
            loaded.Should().NotBeSameAs(poly);
        }

        [Test]
        public void CheckSaveLoad()
        {
            var subType = new PolyTypeBuilder().Save(Repository);

            var entity = new PolyTypeBuilder()
                .With(e => e.SubType, subType)
                .Value();

            entity.CheckSaveLoad(Repository).Check();
        }

        private string GetUnderlyingDbValue(int id)
        {
            var sql = "SELECT [String] FROM [PolyType] WHERE [Id] = :id";
            var query = Repository.Session.CreateSQLQuery(sql);
            query.SetInt32("id", id);
            return query.UniqueResult<string>();
        }
    }
}
