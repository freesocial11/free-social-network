using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Http.Results;
using System.Web.Mvc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Services.Helpers;
using mobSocial.Services.OAuth;
using mobSocial.Services.Security;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Configuration.OAuth;
using mobSocial.WebApi.Helpers;
using mobSocial.WebApi.Models.Applications;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("oauth2")]
    public class OAuthController : RootController
    {
        private readonly IAppTokenService _appTokenService;
        private readonly IApplicationService _applicationService;
        private readonly ICryptographyService _cryptographyService;

        public OAuthController(IAppTokenService appTokenService, IApplicationService applicationService, ICryptographyService cryptographyService)
        {
            _appTokenService = appTokenService;
            _applicationService = applicationService;
            _cryptographyService = cryptographyService;
        }

        [Route("authorize")]
        public ActionResult Authorize()
        {
            if (Response.StatusCode != 200)
            {
                return View("OAuth/AuthorizeError");
            }
            var currentUser = ApplicationContext.Current.CurrentUser;
            if (currentUser == null)
            {
                //challenge the authentication
                ApplicationContext.Current.CurrentOwinContext.Authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }
            var context = ApplicationContext.Current.CurrentOwinContext;
            //let's find out existing scopes / permissions already granted
            //if the requested permissions are same are granted permissions, we just need to simply redirect the user back, 
            //no need for reauthorization

            var scopeParameter = Request.QueryString["scope"];
            var requestedScopes = OAuthScopes.GetEffectiveScopes(scopeParameter);
            if (requestedScopes == null)
            {
                return View("OAuth/AuthorizeError");
            }
            //get the application
            var clientId = Request.QueryString["client_id"];
            var currentToken =
                _appTokenService.FirstOrDefault(x => x.Guid == currentUser.Guid.ToString() && x.ClientId == clientId);
            var skipAuthorizePage = false;

            IEnumerable<OAuthScopes.OAuthScope> allScopes = requestedScopes;
            if (currentToken != null)
            {
                //decrypt the token
                var savedScopes = OAuthHelpers.GetCurrentScopes(currentToken);
                var requestedScopeNames = requestedScopes.Select(x => x.ScopeName);
                //so which are scopes we need to save again?
                var unsavedScopes = requestedScopeNames.Except(savedScopes).ToArray();
                skipAuthorizePage = !unsavedScopes.Any();

                if (!skipAuthorizePage)
                {
                    //we have one or more of additional scopes to be processed
                    var newScopes = OAuthScopes.GetEffectiveScopes(unsavedScopes, savedScopes);
                    requestedScopes = newScopes;
                    allScopes = allScopes.Concat(newScopes);
                }
            }

            if (Request.HttpMethod == "POST" || skipAuthorizePage)
            {
                var authentication = context.Authentication;
                var ticket = authentication.AuthenticateAsync("Application").Result;
                var identity = ticket?.Identity;
                identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                //add the the scopes to new identity
                foreach (var scope in allScopes)
                {
                    identity.AddClaim(new Claim("urn:oauth:scope", scope.ScopeName));
                }
                authentication.SignIn(identity);
                return Content("");
            }

            var application = _applicationService.FirstOrDefault(x => x.Guid == clientId);
            if (application == null)
            {
                return View("OAuth/AuthorizeError");
            }
            //get tokens
            var model = new ApplicationAuthorizeModel()
            {
                Scopes = requestedScopes,
                UserName = ApplicationContext.Current.CurrentUser.Name,
                ApplicationUrl = application.ApplicationUrl,
                RedirectUrl = application.RedirectUrl,
                ApplicationName = application.Name,
                PrivacyPolicyUrl = application.PrivacyPolicyUrl,
                TermsUrl = application.TermsUrl,
                AlreadyAuthorized = skipAuthorizePage
            };

            return View("OAuth/Authorize", model);           
        }

    }
}
