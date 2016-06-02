using System.Data.Entity.Infrastructure;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;

namespace mobSocial.Data.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create()
        {
            var connectionString = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>().ConnectionString;
            return new DatabaseContext(connectionString);
        }
    }
}