using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services;
using mobSocial.Data.Interfaces;
using mobSocial.Services.Extensions;
using mobSocial.Services.Permalinks;

namespace mobSocial.Services
{
    public class MobSocialEntityService<T> : BaseEntityService<T> where T : BaseEntity
    {
        public MobSocialEntityService(IDataRepository<T> dataRepository) : base(dataRepository)
        {
        }

        public override void Insert(T entity)
        {
            base.Insert(entity);
            //insert permalink if its supported
            var supported = entity as IPermalinkSupported;
            if (supported != null)
            {
                supported.GetPermalink();
            }
        }

        public override void Update(T entity)
        {
            /* We should never modify permalinks on updates, even if name has changed.
             * this is because of maintaining seo links, else old links may become 404. 
             * However update functionality can be provided by using webapi*/
            base.Update(entity);
        }

        public override void Delete(T entity)
        {
            var permalinkService = mobSocialEngine.ActiveEngine.Resolve<IPermalinkService>();
            //if it's permalink supported entity then we'll either disable or delete the permalink
            var supported = entity as IPermalinkSupported;
            if (supported != null)
            {
                var permalink = supported.GetPermalink();

                //if the entity is soft deletable, we just disable the permalink
                var deletable = entity as ISoftDeletable;
                if (deletable != null)
                {
                    permalink.Active = false;
                    //update permalink
                    permalinkService.Update(permalink);
                }
                else
                {
                    //we can safely remove the permalink altogether
                    permalinkService.Delete(permalink);
                }
            }

            //call the base entity
            base.Delete(entity);
        }
    }
}