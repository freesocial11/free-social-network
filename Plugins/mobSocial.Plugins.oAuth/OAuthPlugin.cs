using mobSocial.Core.Infrastructure.Mvc;
using mobSocial.Core.Plugins;
using mobSocial.Core.Plugins.Extensibles;
using mobSocial.Data.Database;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.Plugins.OAuth.Migrations;

namespace mobSocial.Plugins.OAuth
{
    //original at http://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/
    public class OAuthPlugin : BasePlugin, IAuthenticationPlugin
    {
        public override RouteData GetConfigurationPageRouteData()
        {
            return null;
        }

        public override RouteData GetDisplayPageRouteData()
        {
            return null;
        }

        public override void Install()
        {
            //set db context to null to avoid any errors
            DatabaseManager.SetDbInitializer<OAuthDbContext>(null);

            //run the migrator. this will update any pending tasks or updates to database
            var migrator = new OAuthDbMigrator(new Configuration());
            migrator.Update();

            base.Install();
        }

        public override void Uninstall()
        {
            //run the migrator. this will remove all the tables created by this migration
            var migrator = new OAuthDbMigrator(new Configuration());
            migrator.Update("0");

            base.Uninstall();
        }
    }
}
