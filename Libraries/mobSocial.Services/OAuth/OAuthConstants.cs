namespace mobSocial.Services.OAuth
{
    public class OAuthConstants
    {
        public const string OAuthBase = "/oauth2";

        public const string TokenEndPointPath = OAuthBase + "/token";

        public const string AuthorizeEndPointPath = OAuthBase + "/authorize";

        public const string LoginPath = "/login";

        public const string LogoutPath = "/logout";

        public const int AccessTokenExpirationSecondsForNativeApplications = 60 /*days*/ * 24 /*hours*/ * 60 /*minutes*/ * 60 /*seconds*/;

        public const int AccessTokenExpirationSecondsForNonNativeApplications = 10 /*minutes*/ * 60 /*seconds*/;

        public const int RefreshTokenExpirationSeconds = 1200;

        public const int AuthorizationCodeExpirationSeconds = 5 * 60; //5 minutes

        public const int ClientSecretLength = 50;
    }
}