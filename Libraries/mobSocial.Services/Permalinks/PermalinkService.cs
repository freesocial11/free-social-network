using mobSocial.Core.Data;
using mobSocial.Data.Entity.Permalinks;
using mobSocial.Data.Interfaces;
using mobSocial.Services.Helpers;

namespace mobSocial.Services.Permalinks
{
    public class PermalinkService : MobSocialEntityService<Permalink>, IPermalinkService
    {
        public PermalinkService(IDataRepository<Permalink> dataRepository) : base(dataRepository)
        {
        }

        public Permalink GetPermalink<T>(T entity) where T : IPermalinkSupported
        {
            //first check if the entity already has a permalink
            var typeName = typeof(T).Name;
            var permalink = FirstOrDefault(x => x.EntityName == typeName && x.EntityId == entity.Id);
            if (permalink == null)
            {
                permalink = new Permalink()
                {
                    EntityName = typeName,
                    EntityId = entity.Id,
                    Slug = PermalinkHelper.GenerateSlug(entity.Name),
                    Active = true
                };
                //save it
                Insert(permalink);
            }
            return permalink;
        }
    }
}