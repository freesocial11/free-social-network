using System.Data.Entity.Infrastructure;

namespace mobSocial.Data.Database.Provider
{
    public class SqlCeDatabaseProvider : IDatabaseProvider
    {
        public IDbConnectionFactory ConnectionFactory
        {
            get
            {
                return new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            }
        }
    }
}