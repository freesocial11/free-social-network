using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Routing;
using System.Web.SessionState;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Configuration.Mvc;
using mobSocial.WebApi.Configuration.Mvc;
using Rhino.Mocks;

namespace mobSocial.Tests.Mock
{
    public static class MockHelper
    {
        public static UrlHelper MockedUrlHelper { get; set; }

        public static HttpConfiguration MockedConfiguration { get; set; }

        /// <summary>
        /// Get mocked http context for testing
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetMockedHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

            return httpContext;
        }

        /// <summary>
        /// Sets up the mock parameters
        /// </summary>
        public static void SetupMockParameters()
        {
            var routes = new RouteCollection();

            var configuration = new HttpConfiguration();
            MockedConfiguration = configuration;

            //first the attribute routes
            configuration.MapHttpAttributeRoutes();

            //register custom routes
            mobSocialEngine.ActiveEngine.Resolve<IRouteMapperService>().MapAllRoutes(routes);
           

            var request = MockRepository.GenerateStrictMock<HttpRequestMessage>();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri("http://localhost/a", UriKind.Absolute);
            //add httpconfiguration to the request
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, configuration);

            var response = MockRepository.GenerateStrictMock<HttpResponseMessage>();
            //response.Stub(x => x.Content).Return("http://localhost/post1");

            /*var context = MockRepository.GenerateStrictMock<HttpContextBase>();
            context.Stub(x => x.Request).Return(request);
            context.Stub(x => x.Response).Return(response);*/

            MockedUrlHelper = new UrlHelper(request);
        }

        public static void MapMvcAttributeRoutesForTesting(this RouteCollection routes)
        {
            var controllers = (from t in typeof(RootApiController).Assembly.GetExportedTypes()
                               where
                                   t != null &&
                                   t.IsPublic &&
                                   t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                                   !t.IsAbstract &&
                                   typeof(IHttpController).IsAssignableFrom(t)
                               select t).ToList();


            var mapMvcAttributeRoutesMethod = typeof(HttpConfigurationExtensions)
                .GetMethod(
                    "MapHttpAttributeRoutes",
                    BindingFlags.Static,
                    null,
                    new[] { typeof(HttpConfiguration) },
                    null);

            mapMvcAttributeRoutesMethod.Invoke(null, new object[] { routes, controllers });
        }
    }
}