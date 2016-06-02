using System.Data.Entity;
using mobSocial.Core.Data;

namespace mobSocial.Data.Database
{
    public interface IDatabaseContext
    {
        IDbSet<T> Set<T>() where T : BaseEntity;

        void ExecuteSql(TransactionalBehavior? transactionalBehavior, string sqlQuery, params object[] parameters);

        int SaveChanges();

        bool DatabaseExists();

        System.Data.Entity.Database Database { get; }
    }
}