using mobSocial.Core.Startup;
using mobSocial.Data.Database;
using mobSocial.Plugins.OAuth.Database;

namespace mobSocial.Plugins.OAuth.Migrations
{
    public class MigrationTask : IStartupTask
    {
        public void Run()
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return;

            //set db context to null to avoid any errors
            DatabaseManager.SetDbInitializer<OAuthDbContext>(null);

            //run the migrator. this will update any pending tasks or updates to database
            var migrator = new OAuthDbMigrator(new Configuration());
            migrator.Update();
        }

        public int Priority => 0;
    }
}