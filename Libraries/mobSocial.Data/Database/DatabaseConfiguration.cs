using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Data.Database
{
    public class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration()
        {
            try
            {
                var databaseProvider = mobSocialEngine.ActiveEngine.Resolve<IDatabaseProvider>();
                SetDefaultConnectionFactory(databaseProvider.ConnectionFactory);
            }
            catch
            {
                SetDefaultConnectionFactory(new SqlConnectionFactory());
            }
           
            
            SetDatabaseInitializer<DatabaseContext>(null);
        }

    }
}