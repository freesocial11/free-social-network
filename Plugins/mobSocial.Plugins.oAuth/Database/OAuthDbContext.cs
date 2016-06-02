using System;
using System.Data.Entity;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Data.Database;
using mobSocial.Data.Entity;

namespace mobSocial.Plugins.OAuth.Database
{
    public class OAuthDbContext : DbContext, IDatabaseContext
    {
        public OAuthDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = typeof(OAuthDbContext).Assembly.GetTypes()
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

        public new IDbSet<T> Set<T>() where T : BaseEntity
        {
            return base.Set<T>();
        }

        public void ExecuteSql(TransactionalBehavior? transactionalBehavior, string sqlQuery, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public bool DatabaseExists()
        {
            return true;
        }
    }
}
