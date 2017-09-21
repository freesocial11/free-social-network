using System.Collections.Generic;
using mobSocial.WebApi.Configuration.OAuth;

namespace mobSocial.WebApi.Models.Applications
{
    public class ApplicationAuthorizeModel
    {
        public IList<OAuthScopes.OAuthScope> Scopes { get; set; }

        public string UserName { get; set; }

        public string ApplicationName { get; set; }

        public string ApplicationUrl { get; set; }

        public string ApplicationLogoUrl { get; set; }

        public string RedirectUrl { get; set; }

        public string TermsUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public bool AlreadyAuthorized { get; set; }
    }
}