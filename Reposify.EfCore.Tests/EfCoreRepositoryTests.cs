using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Reposify.Database.Tests;
using Reposify.Tests;

namespace Reposify.EfCore.Tests
{
    public class EfCoreRepositoryTests : IRepositoryTests
    {
        public class TestsDbContext : DbContext
        {
            public TestsDbContext(string connectionString) : base(connectionString)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

                var polyType = modelBuilder.Entity<PolyType>();
                polyType.HasOptional(p => p.SubType).WithOptionalPrincipal().Map(m => m.MapKey("SubType"));
            }
        }

        private static BuildEnvironment _environment;
        private static EfCoreHandlers   _handlers;

        static EfCoreRepositoryTests()
        {
            _environment = BuildEnvironment.Load();
            _handlers = new EfCoreHandlers().UsingHandlersFromAssemblyForType<EfCoreRepositoryTests>();
        }

        protected override IRepository New()
        {
            var dbContext = new TestsDbContext(_environment.Connection);
            return new EfCoreRepository(dbContext).UsingHandlers(_handlers).Open();
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
