using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.EntityProperties
{
    public class EntityProperty : BaseEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string PropertyName { get; set; }

        public object Value { get; set; }
    }

    public class EntityPropertyMap : BaseEntityConfiguration<EntityProperty> { }
}