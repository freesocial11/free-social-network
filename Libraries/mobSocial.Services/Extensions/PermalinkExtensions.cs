using mobSocial.Core.Infrastructure.AppEngine;
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
    }
}