using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Permalinks;
using mobSocial.Data.Interfaces;
using mobSocial.Services.Permalinks;

namespace mobSocial.Services.Extensions
{
    public static class PermalinkExtensions
    {
        /// <summary>
        /// Gets the permalink for this entity. Automatically creates new if it doesn't exist
        /// </summary>
        /// <param name="permalinkSupportedInstance"></param>
        /// <returns></returns>
        public static Permalink GetPermalink(this IPermalinkSupported permalinkSupportedInstance)
        {
            //resolve permalink service
            var permalinkService = mobSocialEngine.ActiveEngine.Resolve<IPermalinkService>();
            return permalinkService.GetPermalink(permalinkSupportedInstance);
        }

        public static T GetBySeName<T>(this IBaseEntityService<T> entityService, string seName) where T : BaseEntity
        {
            //resolve permalink service
            var permalinkService = mobSocialEngine.ActiveEngine.Resolve<IPermalinkService>();
            var entityname = typeof(T).Name;
            var permalink = permalinkService.FirstOrDefault(x => x.EntityName == entityname && x.Slug == seName && x.Active);
            if (permalink == null)
                return default(T);

            var entityId = permalink.EntityId;
            return entityService.Get(entityId);
        }
    }
}