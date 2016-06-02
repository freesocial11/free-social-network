namespace mobSocial.Plugins.OAuth.Constants
{
    public class OAuthConstants
    {
        public const string OAuthBase = "/oauth2";

        public const string TokenEndPointPath = OAuthBase + "/token";

        public const string AuthorizeEndPointPath = OAuthBase + "/authorize";

        public const string LoginPath = "/login";

        public const string LogoutPath = "/logout";

        public const int AccessTokenExpirationSeconds = 60;

        public const int RefreshTokenExpirationSeconds = 1200;
    }
}