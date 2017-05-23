using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using DryIoc;
using mobSocial.Core;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services;
using mobSocial.Data.Database.Provider;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Integration;

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
            var status = !string.IsNullOrEmpty(dbSettings.ConnectionString) && !string.IsNullOrEmpty(dbSettings.ProviderName);
            if (!status) return false;
            //there is a possibility that tables haven't been installed and we've only connectionstring and provider name.
            //to prevent that, let's check for a table
            return ApplicationHelper.AreTablesInstalled();
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

        public static bool IsMigrationRunning { get; set; } = true;

        public static bool IsDatabaseUpdating { get; set; } = false;
    }
}