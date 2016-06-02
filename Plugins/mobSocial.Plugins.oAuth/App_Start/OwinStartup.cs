using System;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Startup;
using mobSocial.Data.Entity.Settings;
using mobSocial.Plugins.OAuth.Constants;
using mobSocial.Plugins.OAuth.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace mobSocial.Plugins.OAuth
{
    public class OwinStartup : IOwinStartupTask
    {
        private static OAuthBearerAuthenticationOptions options;
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = "Application",
                AuthenticationMode = AuthenticationMode.Active,
                LoginPath = new PathString(OAuthConstants.LoginPath),
                LogoutPath = new PathString(OAuthConstants.LogoutPath),
                CookieManager = new ApplicationCookieManager(),
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = ctx =>
                    {
                        //is ajax request
                        var isRedirectionAllowed = !IsAjaxRequest(ctx.Request);
                        if (!isRedirectionAllowed)
                            return;

                        //if it's an ajax request or it's not an api call 
                        var generalSettings = mobSocialEngine.ActiveEngine.Resolve<GeneralSettings>();
                        //is api call
                        var apiRoot = generalSettings.ApplicationApiRoot;

                        var hostName = (ctx.Request.Headers.Get("Host") ?? "");

                        //oauth root
                        var oAuthRoot = apiRoot + OAuthConstants.AuthorizeEndPointPath;

                        var currentRoot = hostName + ctx.Request.Uri.AbsolutePath; //we are trying to recreate the root from current url and headers

                        isRedirectionAllowed = currentRoot.StartsWith(oAuthRoot); //it's authorization page, so we'll need to redirect

                        if(!isRedirectionAllowed)
                            //so if the current url root starts with apiroot, it's an api end point and redirection is not allowed
                            isRedirectionAllowed = !currentRoot.StartsWith(apiRoot);

                        if (!isRedirectionAllowed)
                            return;

                        //check if it's a login page. for login page, we redirect to a different domain, (the front end application domain i.e.)
                        Uri absoluteUri;
                        if (Uri.TryCreate(ctx.RedirectUri, UriKind.Absolute, out absoluteUri))
                        {
                            var path = PathString.FromUriComponent(absoluteUri);
                            if (path == ctx.OwinContext.Request.PathBase + ctx.Options.LoginPath)
                            {
                                //it's a login path, so let's change the url
                                //TODO: check for https
                                var url = "http://" + generalSettings.ApplicationUiDomain + "/login";
                                ctx.RedirectUri = url + new QueryString(ctx.Options.ReturnUrlParameter, ctx.Request.Uri.AbsoluteUri);
                            }
                                    
                        }

                        ctx.Response.Redirect(ctx.RedirectUri);
                    }
                }
            });

            // Enable External Sign In Cookie
            app.SetDefaultSignInAsAuthenticationType("External");
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = "External",
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = CookieAuthenticationDefaults.CookiePrefix + "External",
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
            });

            options = new OAuthBearerAuthenticationOptions();

            app.UseOAuthBearerAuthentication(options);


            var oAuthServerOptions = new OAuthAuthorizationServerOptions() {

#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString(OAuthConstants.TokenEndPointPath),
                AuthorizeEndpointPath = new PathString(OAuthConstants.AuthorizeEndPointPath),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes((double)OAuthConstants.AccessTokenExpirationSeconds / 60),
                Provider = new ApplicationOAuthServerProvider(),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
                AuthorizationCodeProvider = new ApplicationAuthorizationCodeProvider(),
                ApplicationCanDisplayErrors = true
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);

        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        public int Priority => 0;
    }
}