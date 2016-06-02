using System.Collections.Generic;
using System.Web.Mvc;
using mobSocial.Core.Plugins;

namespace mobSocial.WebApi.Configuration.ViewEngines
{
    public class MobSocialRazorProviderViewEngine : VirtualPathProviderViewEngine
    {
        public MobSocialRazorProviderViewEngine()
        {
            var viewLocations = new List<string>()
            {
                 //default ones first
                "~/Views/{0}.cshtml",
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
            };

            var pluginViewRootFormat = "~/Plugins/{0}/Views/";
            var pluginViewInViewRootFormat = "~/Views/Plugins/{0}/";
            //we make plugin views overridable and searchable
            foreach (var pluginInfo in PluginEngine.Plugins)
            {
                //exclude active plugins
                if(!pluginInfo.Active)
                    continue;

                var pluginSystemName = pluginInfo.SystemName;
                //first in the views folder
                viewLocations.Add(string.Format(pluginViewInViewRootFormat, pluginSystemName) + "{0}.cshtml");
                //then in the plugins folder
                viewLocations.Add(string.Format(pluginViewRootFormat, pluginSystemName) + "{0}.cshtml");

            }
            ViewLocationFormats = viewLocations.ToArray();

            PartialViewLocationFormats = viewLocations.ToArray();

            AreaMasterLocationFormats = new string[]
            {
                
            };
            AreaPartialViewLocationFormats = new string[]
            {

            };
            AreaViewLocationFormats = new string[]
            {
                
            };

            FileExtensions = new string[]
            {
                "cshtml"
            };

            MasterLocationFormats = new string[]
            {
                
            };

        }
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var view = new RazorView(controllerContext, partialPath, null, false, FileExtensions);
            return view;
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var view = new RazorView(controllerContext, viewPath, masterPath, false, FileExtensions);
            return view;
        }
    }
}