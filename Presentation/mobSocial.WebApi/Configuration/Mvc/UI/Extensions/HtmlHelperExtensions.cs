using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.WebApi.Configuration.Mvc.UI.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static void RegisterResource(this HtmlHelper htmlHelper, string resourceName, string resourcePath, ResourceRegistrationType registrationType)
        {
            mobSocialEngine.ActiveEngine.Resolve<IPageGenerator>().RegisterResource(resourceName, resourcePath, registrationType);
        }

        public static void EnqueueStyles(this HtmlHelper htmlHelper, string[] resourceNames, string[] requiredResourceNames = null, ResourcePlacementType placementType = ResourcePlacementType.HeadTag)
        {
            mobSocialEngine.ActiveEngine.Resolve<IPageGenerator>().EnqueueStyles(resourceNames, requiredResourceNames, placementType);
        }

        public static void EnqueueScripts(this HtmlHelper htmlHelper, string[] resourceNames, string[] requiredResourceNames = null, ResourcePlacementType placementType = ResourcePlacementType.BeforeEndBodyTag)
        {
            mobSocialEngine.ActiveEngine.Resolve<IPageGenerator>().EnqueueScripts(resourceNames, requiredResourceNames, placementType);
        }

        public static MvcHtmlString RenderStyles(this HtmlHelper htmlHelper, ResourcePlacementType placementType, bool includeAsBundle = false)
        {
            var pageGenerator = mobSocialEngine.ActiveEngine.Resolve<IPageGenerator>();
            return MvcHtmlString.Create(pageGenerator.RenderStyles(placementType, includeAsBundle));
        }

        public static MvcHtmlString RenderScripts(this HtmlHelper htmlHelper, ResourcePlacementType placementType, bool includeAsBundle = false)
        {
            var pageGenerator = mobSocialEngine.ActiveEngine.Resolve<IPageGenerator>();
            return MvcHtmlString.Create(pageGenerator.RenderScripts(placementType, includeAsBundle));
        }

        public static MvcHtmlString DisplayWidgets(this HtmlHelper htmlHelper, string widgetLocation)
        {
            return htmlHelper.Action("WidgetDisplay", "Widget", new RouteValueDictionary()
            {
                {"widgetLocation", widgetLocation}
            });
        }
    }
}