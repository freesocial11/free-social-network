using mobSocial.Data.Database;

namespace mobSocial.Plugins.OAuth.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<mobSocial.Plugins.OAuth.Database.OAuthDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            TargetDatabase = DatabaseManager.GetDatabaseConnectionInfo();
        }
    }
}
