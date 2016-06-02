using System.Web.Routing;

namespace mobSocial.Core.Infrastructure.Mvc
{
    public interface IRouteMap
    {
        void MapRoutes(RouteCollection routes);
    }
}