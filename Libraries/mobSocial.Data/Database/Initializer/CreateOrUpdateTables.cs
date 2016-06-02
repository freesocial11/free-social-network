using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using mobSocial.Core.Exception;

namespace mobSocial.Data.Database.Initializer
{
    public class CreateOrUpdateTables<TContext> : IDatabaseInitializer<TContext> where TContext:DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            var databaseExists = context.Database.Exists();
            if(!databaseExists)
                throw new mobSocialException("Database not found");

            //create all tables
            var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
            context.Database.ExecuteSqlCommand(dbCreationScript);

            //Seed(context);
            context.SaveChanges();

        }
    }
}