using mobSocial.Core.Data;
using mobSocial.Data.Entity.EntityProperties;

namespace mobSocial.Services.EntityProperties
{
    public class EntityPropertyService : MobSocialEntityService<EntityProperty>, IEntityPropertyService
    {
        public EntityPropertyService(IDataRepository<EntityProperty> dataRepository) : base(dataRepository)
        {
        }
    }
}