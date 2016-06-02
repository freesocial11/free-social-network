using System.Web.Routing;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.UI.Models.Widgets
{
    public class WidgetDisplayModel : RootModel
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public RouteValueDictionary RouteValueDictionary { get; set; }
    }
}