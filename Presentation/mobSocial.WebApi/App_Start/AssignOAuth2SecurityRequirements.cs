using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using mobSocial.WebApi.Configuration.OAuth;
using Swashbuckle.Swagger;

namespace mobSocial.WebApi
{
    internal class AssignOAuth2SecurityRequirements : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var actFilters = apiDescription.ActionDescriptor.GetFilterPipeline();
            var allowsAnonymous = actFilters.Select(f => f.Instance).OfType<OverrideAuthorizationAttribute>().Any();
            if (allowsAnonymous)
                return; // must be an anonymous method


            var scopes = OAuthScopes.AllScopes.Select(x => x.ScopeName).ToList();

            if (operation.security == null)
                operation.security = new List<IDictionary<string, IEnumerable<string>>>();

            var oAuthRequirements = new Dictionary<string, IEnumerable<string>>
            {
                {"oauth2", scopes}
            };

            operation.security.Add(oAuthRequirements);
        }
    }
}