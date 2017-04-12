using System.Data.Entity.Infrastructure;
using mobSocial.Data.Database;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.WebApi.Configuration.Database;

namespace mobSocial.Plugins.OAuth.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<OAuthDbContext>
    {
        public OAuthDbContext Create()
        {
            try
            {
                return DatabaseContextManager.GetDatabaseContext<OAuthDbContext>();

            }
            catch
            {
                return DatabaseContextManager.GetDatabaseContext<OAuthDbContext>(DatabaseManager.FallbackConnectionStringName);
            }
        }
    }
}