using System;
using System.Web;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Startup;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.OAuth.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace mobSocial.Services.OAuth.App_Start
{
    public class OwinStartup : IOwinStartupTask
    {
        public static OAuthBearerAuthenticationOptions BearerOptions;
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

                        var isAuthenticated = ctx.OwinContext.Authentication.User.Identity.IsAuthenticated;
                        if (!isAuthenticated)
                        {
                            //if it's an authorize page request, then only we'll redirect, otherwise, it's best to send the response in json
                            if (ctx.Request.Uri.AbsolutePath.Contains(OAuthConstants.AuthorizeEndPointPath))
                            {
                                ctx.RedirectUri = "/login?ReturnUrl=" + HttpUtility.UrlEncode(ctx.Request.Uri.PathAndQuery);
                                ctx.Response.Redirect(ctx.RedirectUri);
                            }
                        }
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

            BearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new ApplicationBearerAuthenticationProvider()
            };

            app.UseOAuthBearerAuthentication(BearerOptions);


            var oAuthServerOptions = new OAuthAuthorizationServerOptions() {

#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString(OAuthConstants.TokenEndPointPath),
                AuthorizeEndpointPath = new PathString(OAuthConstants.AuthorizeEndPointPath),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes((double)OAuthConstants.AccessTokenExpirationSecondsForNativeApplications / 60),
                Provider = new ApplicationOAuthServerProvider(),
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
                AuthorizationCodeProvider = new ApplicationAuthorizationCodeProvider(),
                ApplicationCanDisplayErrors = true,
                AccessTokenProvider = new ApplicationAccessTokenProvider()
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