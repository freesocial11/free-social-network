using System.Data.Entity.Infrastructure;

namespace mobSocial.Data.Database
{
    public interface IDatabaseProvider
    {
        IDbConnectionFactory ConnectionFactory { get; }
    }
}