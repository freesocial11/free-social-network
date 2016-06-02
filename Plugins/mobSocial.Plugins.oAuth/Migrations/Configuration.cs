using System.Data.Entity.Migrations;
using System.Reflection;
using mobSocial.Plugins.OAuth.Database;

namespace mobSocial.Plugins.OAuth.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<OAuthDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            AutomaticMigrationDataLossAllowed = true;

            MigrationsAssembly = Assembly.GetExecutingAssembly();

            MigrationsNamespace = "mobSocial.Plugins.OAuth.Migrations";

        }
    }
}
