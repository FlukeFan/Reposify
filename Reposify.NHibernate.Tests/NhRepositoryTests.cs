﻿using FluentAssertions;
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
        private static NhHandlers _handlers;

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

            NhRepository.Init(config);
            _handlers = new NhHandlers().UsingHandlersFromAssemblyForType<NhRepositoryTests>();
        }

        private NhRepository _repository;

        protected override IRepository New()
        {
            _repository = new NhRepository().UsingHandlers(_handlers).Open();
            return _repository;
        }

        public override void TearDown()
        {
            using (_repository) { }
            _repository = null;

            base.TearDown();
        }

        [Test]
        public void Flush_UpdatesUnderlyingTable()
        {
            var poly = new PolyTypeBuilder()
                .With(p => p.String, "initial value")
                .Value();

            _repository.Save(poly);
            _repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value");

            Builder.Modify(poly).With(p => p.String, "modified value");

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value", "DB has not been updated");

            _repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("modified value", "in-memory changes have been flushed to the DB");
        }

        [Test]
        public void Clear_PurgesIdentityMap()
        {
            var poly = new PolyTypeBuilder()
                .With(p => p.String, "initial value")
                .Value();

            _repository.Save(poly);
            _repository.Flush();

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value");

            Builder.Modify(poly).With(p => p.String, "modified value");

            GetUnderlyingDbValue(poly.Id).Should().Be("initial value", "DB has not been updated");

            _repository.Clear();

            var loaded = _repository.Load<PolyType>(poly.Id);

            loaded.String.Should().Be("initial value");
            loaded.Should().NotBeSameAs(poly);
        }

        [Test]
        public void CheckSaveLoad()
        {
            var repository = New();
            var subType = new PolyTypeBuilder().Save(repository);

            var entity = new PolyTypeBuilder()
                .With(e => e.SubType, subType)
                .Value();

            entity.CheckSaveLoad(repository).Check();
        }

        private string GetUnderlyingDbValue(int id)
        {
            var sql = "SELECT [String] FROM [PolyType] WHERE [Id] = :id";
            var query = _repository.Session.CreateSQLQuery(sql);
            query.SetInt32("id", id);
            return query.UniqueResult<string>();
        }
    }
}
