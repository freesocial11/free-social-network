using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.Settings;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;

namespace mobSocial.Plugins.OAuth.Provider
{
    /// <summary>
    /// Just to modify the default cookie domain, we had to create this class
    /// </summary>
    public class ApplicationCookieManager : ICookieManager
    {
        private readonly ICookieManager _defaultCookieManager;

        public ApplicationCookieManager()
        {
            _defaultCookieManager = new ChunkingCookieManager();
        }
        public string GetRequestCookie(IOwinContext context, string key)
        {
            return _defaultCookieManager.GetRequestCookie(context, key);
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
        {
            SetupDomain(context, options);
            _defaultCookieManager.AppendResponseCookie(context, key, value, options);
        }

        public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
        {
            SetupDomain(context, options);
            _defaultCookieManager.DeleteCookie(context, key, options);
        }

        private void SetupDomain(IOwinContext context, CookieOptions options)
        {
            //here we assign the cookie domain, so first resolve our general settings
            var generalSettings = mobSocialEngine.ActiveEngine.Resolve<GeneralSettings>();
            options.Domain = generalSettings.ApplicationCookieDomain;
        }
    }
}
