using System;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Web;
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
            var dbContext =  new DatabaseContext(connectionString);
            return dbContext;
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

        public static T GetDatabaseContext<T>(string nameOrConnectionString) where T : DbContext
        {
            return (T)Activator.CreateInstance(typeof(T), nameOrConnectionString);
        }
    }
}