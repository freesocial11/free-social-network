using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.MediaEntities
{
    public class EntityMedia : BaseEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int MediaId { get; set; }
    }

    public class EntityMediaMap: BaseEntityConfiguration<EntityMedia> { }
}
