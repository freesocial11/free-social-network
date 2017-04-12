using System.Data.Entity.Infrastructure;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;

namespace mobSocial.Data.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create()
        {
            try
            {
                var connectionString = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>().ConnectionString;
                return new DatabaseContext(connectionString);
            }
            catch
            {
                return new DatabaseContext(DatabaseManager.FallbackConnectionStringName); //try if there is one in web.config
            }
        }
    }
}