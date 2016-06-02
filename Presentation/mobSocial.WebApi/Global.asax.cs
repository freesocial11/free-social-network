using System.Runtime.CompilerServices;
using System.Web.Mvc;
using System.Web.Routing;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.WebApi.Configuration.ViewEngines;

[assembly: InternalsVisibleTo("mobSocial.Tests")]
namespace mobSocial.WebApi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //setup initial tasks
            mobSocialEngine.ActiveEngine.Start();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //setup view engines
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MobSocialRazorProviderViewEngine());

        }
    }
}
