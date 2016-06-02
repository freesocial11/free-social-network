using System.Data.Entity;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Data.Database
{
    public class DatabaseManager
    {
        
        public static void SetDbInitializer<T>(IDatabaseInitializer<T> initializer) where T : DbContext
        {
            System.Data.Entity.Database.SetInitializer(initializer);
        }

        public static bool IsDatabaseInstalled()
        {
            var dbSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            return !string.IsNullOrEmpty(dbSettings.ConnectionString) && !string.IsNullOrEmpty(dbSettings.ProviderName);
        }

        
    }
}