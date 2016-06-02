using System.Web.Routing;

namespace mobSocial.Core.Infrastructure.Mvc
{
    public partial class RouteData
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public dynamic RouteValueDictionary { get; set; }

        public RouteValueDictionary GetRouteValueDictionary()
        {
            return (RouteValueDictionary) RouteValueDictionary;
        }
    }
}