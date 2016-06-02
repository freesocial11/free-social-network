using mobSocial.Core.Startup;
using mobSocial.Data.Database;
using mobSocial.Data.Migrations;

namespace mobSocial.WebApi.Configuration.Tasks
{
    public class DatabaseSetupTask : IStartupTask
    {

        public void Run()
        {

            if (!DatabaseManager.IsDatabaseInstalled())
                return;

            //set db context to null to avoid any errors
            DatabaseManager.SetDbInitializer<DatabaseContext>(null);

            //run the migrator. this will update any pending tasks or updates to database
            var migrator = new mobSocialDbMigrator(new Data.Migrations.Configuration());
            migrator.Update();
        }

        public int Priority
        {
            get { return -int.MaxValue; } //should be the first task ever
        }
    }
}