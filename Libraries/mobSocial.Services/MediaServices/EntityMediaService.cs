using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.MediaEntities;

namespace mobSocial.Services.MediaServices
{
    public class EntityMediaService : BaseEntityService<EntityMedia>, IEntityMediaService
    {
        public EntityMediaService(IDataRepository<EntityMedia> dataRepository) : base(dataRepository) {}
    }
}