using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace mobSocial.WebApi.Configuration.Routing
{
    public class CentralizedPrefixProvider : DefaultDirectRouteProvider
    {
        private readonly string _centralizedPrefix;

        public CentralizedPrefixProvider(string centralizedPrefix)
        {
            _centralizedPrefix = centralizedPrefix;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            if (existingPrefix == null) return _centralizedPrefix;
            if(string.IsNullOrEmpty(_centralizedPrefix))
                return existingPrefix;

            return $"{_centralizedPrefix}/{existingPrefix}";
        }
    }
}