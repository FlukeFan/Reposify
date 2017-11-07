using System.Data;
using System.Data.SqlClient;
using FluentAssertions;
using NUnit.Framework;

namespace Reposify.Database.Tests
{
    [TestFixture]
    public class CreateDatabaseTests
    {
        private static BuildEnvironment _environment = BuildEnvironment.Load();

        private const string DropDb = @"
            IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Reposify')
            BEGIN
                ALTER DATABASE Reposify
                    SET SINGLE_USER
                    WITH ROLLBACK IMMEDIATE

                DROP DATABASE Reposify
            END";

        private const string CreateDb = "Create Database Reposify";

        private const string CreateStructure = @"
            CREATE TABLE [dbo].[PolyType](
                [Id]                [int] IDENTITY(1,1) NOT NULL,
                [String]            [varchar](255)      NOT NULL,
                [BigString]         [varchar](max)      NOT NULL,
                [Int]               [int]               NOT NULL,
                [Boolean]           [bit]               NOT NULL,
                [DateTime]          [datetime]          NOT NULL,
                [Enum]              [int]               NOT NULL,
                [NullableInt]       [int]               NULL,
                [NullableBoolean]   [bit]               NULL,
                [NullableDateTime]  [datetime]          NULL,
                [NullableEnum]      [int]               NULL,
                [SubType]           [int]               NULL,
             CONSTRAINT [PK_PolyType_Id] PRIMARY KEY CLUSTERED
            (
                [Id] ASC
            ))
            ";

        [Test]
        [Explicit("Run when cleaning the build")]
        public void DropDatabase()
        {
            using (var masterDb = new SqlConnection(_environment.MasterConnection))
            {
                masterDb.Open();
                ExecuteNonQuery(masterDb, DropDb);
            }
        }

        [Test]
        public void CreateDatabase()
        {
            DropAndCreateBlankDb();

            using (var db = new SqlConnection(_environment.Connection))
            {
                db.Open();
                ExecuteNonQuery(db, CreateStructure);
            }

            using (var db = new SqlConnection(_environment.Connection))
            {
                db.Open();

                var cmd = db.CreateCommand();
                cmd.CommandText = "select count(*) from [PolyType]";
                var rows = (int)cmd.ExecuteScalar();

                rows.Should().Be(0);
            }
        }

        private void ExecuteNonQuery(IDbConnection connection, string sql)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private void DropAndCreateBlankDb()
        {
            using (var masterDb = new SqlConnection(_environment.MasterConnection))
            {
                masterDb.Open();
                ExecuteNonQuery(masterDb, DropDb);
                ExecuteNonQuery(masterDb, CreateDb);
            }
        }
    }
}
