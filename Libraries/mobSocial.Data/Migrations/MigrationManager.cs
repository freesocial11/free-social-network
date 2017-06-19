using mobSocial.Data.Database;
using System.Linq;

namespace mobSocial.Data.Migrations
{
    public static class MigrationManager
    {
        public static void UpdateDatabaseToLatestVersion()
        {
            DatabaseManager.IsDatabaseUpdating = true;
            var migrator = new mobSocialDbMigrator(new Configuration());
            if (migrator.GetPendingMigrations().Any())
                migrator.Update();

            DatabaseManager.IsDatabaseUpdating = false;
        }
    }
}
