using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Reposify.Database.Tests;
using Reposify.Tests;

namespace Reposify.EfCore.Tests
{
    [TestFixture]
    public class EfCoreRepositoryTests : IRepositoryTests
    {
        public class TestsDbContext : DbContext
        {
            public TestsDbContext(string connectionString) : base(BuildOptions(connectionString))
            {
            }

            public static DbContextOptions BuildOptions(string connectionString)
            {
                return new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString)
                    .Options;
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var polyType = modelBuilder.Entity<PolyType>();
                polyType.HasOne(e => e.SubType);
                var props = polyType.Metadata.GetProperties().ToList();
                props.Where(p => p.Name == "SubTypeId").Single().Relational().ColumnName = "SubType";
            }
        }

        private static BuildEnvironment _environment;
        private static EfCoreHandlers   _handlers;

        static EfCoreRepositoryTests()
        {
            _environment = BuildEnvironment.Load();
            _handlers = new EfCoreHandlers().UsingHandlersFromAssemblyForType<EfCoreRepositoryTests>();
        }

        public static EfCoreRepository NewEfCoreRepository()
        {
            var dbContext = new TestsDbContext(_environment.Connection);
            return new EfCoreRepository(dbContext).UsingHandlers(_handlers).Open();
        }

        protected override IDisposable New()
        {
            return EfCoreRepositoryTests.NewEfCoreRepository();
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
