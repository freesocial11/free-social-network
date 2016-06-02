using System.Web.Http;
using DryIoc.WebApi;
using mobSocial.Core.Infrastructure.AppEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace mobSocial.WebApi
{
    public class WebApiConfig
    {
        private static HttpConfiguration _httpConfiguration;

        public static HttpConfiguration Configuration
        {
            get { return _httpConfiguration; }
            set { _httpConfiguration = value; }
        }
        public static void Register(HttpConfiguration configuration)
        {
            _httpConfiguration = configuration;

            //setup attribute routes
            configuration.MapHttpAttributeRoutes();

            configuration.Routes.MapHttpRoute
                (
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new {id = RouteParameter.Optional}
                );

            
            //dependency resolver
            mobSocialEngine.ActiveEngine.IocContainer.WithWebApi(configuration);

            var formatter = configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            formatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            
        }

    }
}