using System;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;

namespace mobSocial.WebApi.Configuration.Database
{
    public class DatabaseContextManager
    {
        public static DatabaseContext GetDatabaseContext()
        {
            var databaseSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            var connectionString = databaseSettings.ConnectionString;
            return new DatabaseContext(connectionString);
        }

        public static DatabaseContext GetDatabaseContext(string connectionString)
        {
            return new DatabaseContext(connectionString);
        }

        public static T GetDatabaseContext<T>() where T: IDatabaseContext
        {
            var databaseSettings = mobSocialEngine.ActiveEngine.Resolve<IDatabaseSettings>();
            var connectionString = databaseSettings.ConnectionString;
            return (T) Activator.CreateInstance(typeof(T), connectionString);
        }
    }
}