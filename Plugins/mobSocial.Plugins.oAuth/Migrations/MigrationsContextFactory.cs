using System.Data.Entity.Infrastructure;
using mobSocial.Plugins.OAuth.Database;
using mobSocial.WebApi.Configuration.Database;

namespace mobSocial.Plugins.OAuth.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<OAuthDbContext>
    {
        public OAuthDbContext Create()
        {
            return DatabaseContextManager.GetDatabaseContext<OAuthDbContext>();
        }
    }
}