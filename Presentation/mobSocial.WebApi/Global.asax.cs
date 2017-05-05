using System;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.WebApi.Configuration.ViewEngines;

[assembly: InternalsVisibleTo("mobSocial.Tests")]
namespace mobSocial.WebApi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();

            //setup initial tasks
            mobSocialEngine.ActiveEngine.Start();

            //specify that migrations are not running and it's the app that's working
            DatabaseManager.IsMigrationRunning = false;

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //setup view engines
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MobSocialRazorProviderViewEngine());

        }
    }
}
