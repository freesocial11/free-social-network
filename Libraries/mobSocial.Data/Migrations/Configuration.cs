using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Reflection;
using mobSocial.Data.Database;

namespace mobSocial.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            AutomaticMigrationDataLossAllowed = true;

            MigrationsAssembly = Assembly.GetExecutingAssembly();

            MigrationsNamespace = "mobSocial.Data.Migrations";

        }

        public Configuration(string connectionString, string providerName) : this()
        {
            TargetDatabase = new DbConnectionInfo(connectionString, providerName);
        }
    }
}