using System.Collections.Generic;
using System.Linq;

namespace mobSocial.WebApi.Configuration.OAuth
{
    public static class OAuthScopes
    {
        public class OAuthScope
        {
            public string ScopeName { get; set; }

            public string Description { get; set; }

            public static OAuthScope NewScope(string scopeName, string description)
            {
                return new OAuthScope()
                {
                    ScopeName = scopeName,
                    Description = description
                };
            }
        }

        public const string DefaultScope = "basicinfo-r";
        public const string FullPermissionScope = "everything-rwd";

        private static readonly Dictionary<string, string> ScopeList = new Dictionary<string, string>()
        {
            {DefaultScope, "Read your basic information such as First Name and Last Name" },
            {"email-r", "Read your email address"},
            {"dob-r", "Read your date of birth"},
            {"friends-r", "Get list of your friends"},
            {"friends-rw", "Add a user as your friend"},
            {"friends-rwd", "View and manage your friends"},
            {"followers-r", "Get list of your followers"},
            {"followers-rw", "Add a user as a follow"},
            {"followers-rwd", "View and manage your followers"},
            {"timeline-r", "Read your timeline posts"},
            {"timeline-rw", "Write new posts to your timeline"},
            {"timeline-rwd", "View and manage your timeline"},
            {"notifications-r", "Read your recent notifications"},
            {"notifications-rw", "Write notifications for you"},
            {"notifications-rwd", "View and manage your notifications"},
            {"skills-r", "Read your skills"},
            {"skills-rw", "Write and modify your skills"},
            {"skills-rwd", "View and manage your skills"},
            {"profile-rw", "Modify your profile information except Email"},
            {"conversations-r", "Read your conversations"},
            {"conversations-rw", "Write your conversations and messages"},
            {"conversations-rwd", "View and manage your conversations"},
            {"educations-r", "Read your education info"},
            {"educations-rw", "Modify your education info"},
            {"educations-rwd", "View and manage your educations"},
            {"pa-r", "Read your private attributes e.g. Size, Favorite Color etc."},
            {"pa-rw", "Write new private attributes or overwrite previous attributes"},
            {"pa-rwd", "View and manage your private attributes"},
            {FullPermissionScope, "Do anything on your behalf"},

        };

        public static readonly IList<OAuthScope> AllScopes;
        static OAuthScopes()
        {
            AllScopes = new List<OAuthScope>();
            foreach (var scope in ScopeList)
            {
                AllScopes.Add(OAuthScope.NewScope(scope.Key, scope.Value));
            }
        }

        public static bool TryGet(string scopeName, out OAuthScope scope)
        {
            scope = AllScopes.FirstOrDefault(x => x.ScopeName == scopeName);
            return scope != null;
        }

        public static OAuthScope Get(string scopeName)
        {
            return AllScopes.First(x => x.ScopeName == scopeName);
        }

        public static IList<OAuthScope> GetEffectiveScopes(string scopeNamesQueryString, string[] excludeScopes = null)
        {
            if (string.IsNullOrEmpty(scopeNamesQueryString))
                return new List<OAuthScope>() {Get(DefaultScope)};

            var scopeNames = scopeNamesQueryString.Split(',');
            return GetEffectiveScopes(scopeNames, excludeScopes);
        }

        public static IList<OAuthScope> GetEffectiveScopes(string[] scopeNames, string[] excludeScopes = null)
        {
            if (scopeNames.Any(x => x == FullPermissionScope))
                return new List<OAuthScope>() { Get(FullPermissionScope) };

            var effectiveScopeNames = new List<string>();
            var allowedPermissions = new[] { "r", "rw", "rwd" };
            foreach (var scopeName in scopeNames)
            {
                var scopeParts = scopeName.Split('-');
                if (scopeParts.Length < 2)
                    return null; //there is something wrong with the passed scope terminating...

                var featureName = scopeParts[0];
                var permissions = scopeParts[1];
                if (!allowedPermissions.Contains(permissions))
                    return null;
                switch (permissions)
                {
                    case "r":
                        if (effectiveScopeNames.Any(x => x == $"{featureName}-rw" || x == $"{featureName}-rwd"))
                        {
                            continue;
                        }
                        effectiveScopeNames.Add(scopeName);
                        break;
                    case "rw":
                        if (effectiveScopeNames.Any(x => x == $"{featureName}-rwd"))
                        {
                            continue;
                        }
                        effectiveScopeNames.Remove($"{featureName}-r"); //remove the read-only one
                        effectiveScopeNames.Add(scopeName);
                        break;
                    case "rwd":
                        effectiveScopeNames.Remove($"{featureName}-r"); //remove the read-only one
                        effectiveScopeNames.Remove($"{featureName}-rw"); //remove the read-write one
                        effectiveScopeNames.Add(scopeName);
                        break;
                    default:
                        return null;
                }

            }

            var scopes = new List<OAuthScope>();
            //add basic info in any case
            if (effectiveScopeNames.All(x => x != DefaultScope))
            {
                if(excludeScopes == null || !excludeScopes.Contains(DefaultScope))
                    scopes.Add(Get(DefaultScope));
            }

            foreach (var esn in effectiveScopeNames)
            {
                if (excludeScopes != null && excludeScopes.Contains(esn))
                    continue;
                if (TryGet(esn, out OAuthScope scopeObject))
                {
                    scopes.Add(scopeObject);
                }
                else
                {
                    return null; //we didn't find this scope, there is something wrong here
                }

            }
            return scopes;
        }

        public static IEnumerable<OAuthScope> GetScopes(IEnumerable<string> scopeNames)
        {
            return AllScopes.Where(x => scopeNames.Contains(x.ScopeName));
        }
    }
}