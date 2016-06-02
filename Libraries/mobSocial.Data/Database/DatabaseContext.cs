using System;
using System.Data.Entity;
using System.Linq;
using mobSocial.Data.Entity;

namespace mobSocial.Data.Database
{
    //[DbConfigurationType("mobSocial.Data.Database.DatabaseConfiguration, mobSocial.Data")]
    public class DatabaseContext : DbContext, IDatabaseContext
    {
       
        public DatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            var typesToRegister = typeof(DatabaseContext).Assembly.GetTypes()
          .Where(type => !string.IsNullOrEmpty(type.Namespace))
          .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
              type.BaseType.GetGenericTypeDefinition() == typeof(BaseEntityConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

        public void ExecuteSql(TransactionalBehavior? transactionalBehavior, string sqlQuery, params object[] parameters)
        {
            if (transactionalBehavior.HasValue)
                this.Database.ExecuteSqlCommand(transactionalBehavior.Value, sqlQuery, parameters);
            else
                this.Database.ExecuteSqlCommand(sqlQuery, parameters);
            
        }

        public new IDbSet<T> Set<T>() where T : Core.Data.BaseEntity
        {
            return base.Set<T>();
        }


        public bool DatabaseExists()
        {
            return Database.Exists();
        }
    }
}