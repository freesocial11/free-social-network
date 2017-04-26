using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database.Provider;

namespace mobSocial.Data.Database
{
    public class DatabaseManager
    {
        public const string FallbackConnectionStringName = "migrationConnectionString";

        public static void SetDbInitializer<T>(IDatabaseInitializer<T> initializer) where T : DbContext
        {
            System.Data.Entity.Database.SetInitializer(initializer);
        }

        public static bool IsDatabaseInstalled()
        {
            var dbSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            return !string.IsNullOrEmpty(dbSettings.ConnectionString) && !string.IsNullOrEmpty(dbSettings.ProviderName);
        }

        public static string GetProviderName(string providerAbstractName)
        {
            switch (providerAbstractName.ToLower())
            {

                case "sqlserver":
                    return "System.Data.SqlClient";
                case "sqlce":
                    return "System.Data.SqlServerCe.4.0";
            }
            return providerAbstractName;
        }

        /// <summary>
        /// Checks if specified connectionstring is valid and works
        /// </summary>
        public static bool DatabaseConnects(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return false;
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();
                    con.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates connection string from the provider values
        /// </summary>
        public static string CreateConnectionString(string server, string databaseName, string userName, string password, bool integratedSecurity, int timeOut)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder {
                    IntegratedSecurity = integratedSecurity,
                    DataSource = server,
                    InitialCatalog = databaseName
                };
                if (!integratedSecurity)
                {
                    builder.UserID = userName;
                    builder.Password = password;
                }
                builder.PersistSecurityInfo = false;
                if (timeOut > 0)
                {
                    builder.ConnectTimeout = timeOut;
                }
                return builder.ConnectionString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DbConnectionInfo GetDatabaseConnectionInfo()
        {
            try
            {
                var dbSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
                return new DbConnectionInfo(dbSettings.ConnectionString, GetProviderName(dbSettings.ProviderName));
            }
            catch
            {
                return new DbConnectionInfo(FallbackConnectionStringName);
            }
        }
 
    }
}