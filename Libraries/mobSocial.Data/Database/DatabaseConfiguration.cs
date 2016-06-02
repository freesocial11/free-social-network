using System.Data.Entity;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Data.Database
{
    public class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration()
        {
            var databaseProvider = mobSocialEngine.ActiveEngine.Resolve<IDatabaseProvider>();

            SetDefaultConnectionFactory(databaseProvider.ConnectionFactory);
            
            SetDatabaseInitializer<DatabaseContext>(null);
        }

    }
}