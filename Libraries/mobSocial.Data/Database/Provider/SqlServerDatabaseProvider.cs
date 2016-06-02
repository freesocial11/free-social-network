using System.Data.Entity.Infrastructure;

namespace mobSocial.Data.Database.Provider
{
    public class SqlServerDatabaseProvider : IDatabaseProvider
    {

        public IDbConnectionFactory ConnectionFactory
        {
            get
            {
                return new SqlConnectionFactory();
            }
        }
    }
}