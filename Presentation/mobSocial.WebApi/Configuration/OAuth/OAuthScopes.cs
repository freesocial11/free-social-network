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
        public const string EmailR = "email-r";
        public const string DobR = "dob-r";
        public const string FriendsR = "friends-r";
        public const string FriendsRW = "friends-rw";
        public const string FriendsRWD = "friends-rwd";
        public const string FollowersR = "followers-r";
        public const string FollowersRW = "followers-rw";
        public const string FollowersRWD = "followers-rwd";
        public const string TimelineR = "timeline-r";
        public const string TimelineRW = "timeline-rw";
        public const string TimelineRWD = "timeline-rwd";
        public const string CommentsR = "comments-r";
        public const string CommentsRW = "comments-rw";
        public const string CommentsRWD = "comments-rwd";
        public const string NotificationsR = "notifications-r";
        public const string NotificationsRW = "notifications-rw";
        public const string NotificationsRWD = "notifications-rwd";
        public const string SkillsR = "skills-r";
        public const string SkillsRW = "skills-rw";
        public const string SkillsRWD = "skills-rwd";
        public const string ProfileR = "profile-r";
        public const string ProfileRW = "profile-rw";
        public const string ConversationsR = "conversations-r";
        public const string ConversationsRW = "conversations-rw";
        public const string ConversationsRWD = "conversations-rwd";
        public const string EducationsR = "educations-r";
        public const string EducationsRW = "educations-rw";
        public const string EducationsRWD = "educations-rwd";
        public const string PrivateAttributesR = "private-attributes-r";
        public const string PrivateAttributesRW = "private-attributes-rw";
        public const string PrivateAttributesRWD = "private-attributes-rwd";
        public const string ApplicationsR = "applications-r";
        public const string ApplicationsRW = "applications-rw";
        public const string ApplicationsRWD = "applications-rwd";
        public const string EntityPropertyRWD = "entity-property-rwd";
        public const string FollowUnfollow = "follow-unfollow-rwd";
        public const string LikeUnlike = "like-unlike-rwd";
        public const string MediaR = "media-r";
        public const string MediaRW = "media-rw";
        public const string MediaRWD = "media-rwd";
        public const string ProfilePwdW = "pwd-rwd";

        private static readonly Dictionary<string, string> ScopeList = new Dictionary<string, string>()
        {
            {DefaultScope, "Read your basic information such as First Name and Last Name" },
            {EmailR, "Read your email address"},
            {DobR, "Read your date of birth"},
            {FriendsR, "Get list of your friends"},
            {FriendsRW, "Add a user as your friend"},
            {FriendsRWD, "View and manage your friends"},
            {FollowersR, "Get list of your followers"},
            {FollowersRW, "Add a user as a follow"},
            {FollowersRWD, "View and manage your followers"},
            {TimelineR, "Read your timeline posts"},
            {TimelineRW, "Write new posts to your timeline"},
            {TimelineRWD, "View and manage your timeline"},
            {NotificationsR, "Read your recent notifications"},
            {NotificationsRW, "Write notifications for you"},
            {NotificationsRWD, "View and manage your notifications"},
            {CommentsR, "Read your comments"},
            {CommentsRW, "Write comments on your behalf"},
            {CommentsRWD, "View and manage your comments"},
            {SkillsR, "Read your skills"},
            {SkillsRW, "Write and modify your skills"},
            {SkillsRWD, "View and manage your skills"},
            {ProfileRW, "Modify your profile information except Email"},
            {ProfilePwdW, "Modify your profile password"},
            {ConversationsR, "Read your conversations"},
            {ConversationsRW, "Write your conversations and messages"},
            {ConversationsRWD, "View and manage your conversations"},
            {EducationsR, "Read your education info"},
            {EducationsRW, "Modify your education info"},
            {EducationsRWD, "View and manage your educations"},
            {PrivateAttributesR, "Read your private attributes e.g. Size, Favorite Color etc."},
            {PrivateAttributesRW, "Write new private attributes or overwrite previous attributes"},
            {PrivateAttributesRWD, "View and manage your private attributes"},
            {ApplicationsR, "Read your OAuth applications"},
            {ApplicationsRW, "Read, Create or Modify your OAuth applications"},
            {ApplicationsRWD, "View and manage your OAuth applications"},
            {MediaR, "Read your media"},
            {MediaRW, "Read, Create or Modify your media"},
            {MediaRWD, "View and manage your media"},
            {EntityPropertyRWD, "View and manage custom properties for resources"},
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
                if (string.IsNullOrEmpty(scopeName))
                    continue;
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