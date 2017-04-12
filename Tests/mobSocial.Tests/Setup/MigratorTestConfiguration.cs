using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using mobSocial.Data.Database;

namespace mobSocial.Tests.Setup
{
    public class MigratorTestConfiguration : DbMigrationsConfiguration<DatabaseContext>
    {
        public MigratorTestConfiguration(string connectionString, string providerName)
        {
            AutomaticMigrationsEnabled = true;

            TargetDatabase = new DbConnectionInfo(connectionString, providerName);

        }
    }
}