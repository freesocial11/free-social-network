using System.Web.Mvc;
using System.Web.Routing;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Configuration.Mvc;

namespace mobSocial.WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //first the attribute routes
            routes.MapMvcAttributeRoutes();

            //register custom routes
            mobSocialEngine.ActiveEngine.Resolve<IRouteMapperService>().MapAllRoutes(routes);

            //a default route as well
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}