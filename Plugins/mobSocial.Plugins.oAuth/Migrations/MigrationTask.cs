using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Startup;
using mobSocial.Data.Database;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.Services.Plugins;

namespace mobSocial.Plugins.OAuth.Migrations
{
    public class MigrationTask : IStartupTask
    {
        public void Run()
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return;

            //check if the plugin is installed or not?
            var pluginFinder = mobSocialEngine.ActiveEngine.Resolve<IPluginFinderService>();
            if (pluginFinder.FindPlugin("mobSocial.Plugins.oAuth") == null)
            {
                return; //plugin is not installed
            }
            //set db context to null to avoid any errors
            DatabaseManager.SetDbInitializer<OAuthDbContext>(null);

            //run the migrator. this will update any pending tasks or updates to database
            var migrator = new OAuthDbMigrator(new Configuration());
            migrator.Update();
        }

        public int Priority => 0;
    }
}