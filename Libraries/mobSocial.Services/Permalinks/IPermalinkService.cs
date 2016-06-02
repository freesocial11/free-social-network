using mobSocial.Core.Services;
using mobSocial.Data.Entity.Permalinks;
using mobSocial.Data.Interfaces;

namespace mobSocial.Services.Permalinks
{
    public interface IPermalinkService : IBaseEntityService<Permalink>
    {
        /// <summary>
        /// Gets the permalink for specified entity. Saves a new if not found and returns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Permalink GetPermalink<T>(T entity) where T : IPermalinkSupported;
    }
}