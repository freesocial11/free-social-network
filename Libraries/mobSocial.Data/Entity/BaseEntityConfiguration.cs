using System.Data.Entity.ModelConfiguration;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity
{
    public abstract class BaseEntityConfiguration<T> : EntityTypeConfiguration<T> where T : BaseEntity
    {
        protected virtual string TableName { get { return MappingConfiguration.TablePrefix + typeof (T).Name; } }

        protected BaseEntityConfiguration()
        {
            ToTable(this.TableName);
        } 
    }
}