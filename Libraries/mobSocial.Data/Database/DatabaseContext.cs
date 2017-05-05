using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using mobSocial.Core.Data;
using mobSocial.Data.Database.Attributes;
using mobSocial.Data.Entity;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Database
{
    //[DbConfigurationType("mobSocial.Data.Database.DatabaseConfiguration, mobSocial.Data")]
    public class DatabaseContext : DbContext, IDatabaseContext
    {
       
        public DatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            DatabaseManager.SetDbInitializer<DatabaseContext>(null);

            var typesToRegister = typeof(DatabaseContext).Assembly.GetTypes()
          .Where(type => !string.IsNullOrEmpty(type.Namespace))
          .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
              type.BaseType.GetGenericTypeDefinition() == typeof(BaseEntityConfiguration<>));

            foreach (var type in typesToRegister)
            {
               
                dynamic configurationInstance = Activator.CreateInstance(type);
               
                //check if entity uses views at runtime e.g. User table
                var runTimeViewAttribute = type.GetCustomAttributes(typeof(ToRunTimeViewAttribute), true)
                    .FirstOrDefault() as ToRunTimeViewAttribute;
                if (runTimeViewAttribute != null && !DatabaseManager.IsMigrationRunning && !DatabaseManager.IsDatabaseUpdating)
                {
                    MethodInfo toTableMethod = configurationInstance.GetType().GetMethod("ToTable", new[] { typeof(string) });
                    toTableMethod.Invoke(configurationInstance, new object[] {runTimeViewAttribute.ViewName});
                }

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