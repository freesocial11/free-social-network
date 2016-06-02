using System.Web;
using System.Web.Routing;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.Services.Permalinks;

namespace mobSocial.WebApi.Configuration.Routing
{
    public class GenericRoute : Route
    {
        public GenericRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler)
        {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);
            if (DatabaseManager.IsDatabaseInstalled() && routeData != null)
            {
                var permalinkService = mobSocialEngine.ActiveEngine.Resolve<IPermalinkService>();

                var slug = routeData.Values["sename"];
                if(slug == null)
                    return routeData;
                
                var permalink = permalinkService.FirstOrDefault(x => x.Slug == slug.ToString());
                if (permalink == null || !permalink.Active)
                {
                    routeData.Values["action"] = "PageNotFound";
                    routeData.Values["controller"] = "Common";
                    return routeData;;
                }

                var entityId = permalink.EntityId;
                var entityName = permalink.EntityName;
                routeData.Values["id"] = entityId;
                switch (entityName)
                {
                    case "VideoBattle":
                        routeData.Values["controller"] = "VideoBattle";
                        routeData.Values["action"] = "Get";
                        break;
                    case "User":
                        routeData.Values["controller"] = "User";
                        routeData.Values["action"] = "Get";
                        break;
                }
            }
            return routeData;
            
        }
    }
}