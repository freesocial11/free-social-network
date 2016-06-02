using System;
using System.Web.Mvc;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Configuration.Infrastructure;

namespace mobSocial.WebApi.Configuration.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CapabilityRequiredAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly string[] _capabilityName;

        public CapabilityRequiredAttribute(params string[] capabilityName)
        {
            _capabilityName = capabilityName;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if(filterContext == null)
                throw new ArgumentNullException();

            //get the current user
            var currentUser = ApplicationContext.Current.CurrentUser;

            if(!currentUser.Can(_capabilityName))
                filterContext.Result = new HttpUnauthorizedResult();

        }
    }
}