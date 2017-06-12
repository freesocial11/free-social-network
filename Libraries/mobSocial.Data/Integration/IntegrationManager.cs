using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Integration
{
    public class IntegrationManager
    {
        internal static IIntegrationMap<UserMap> UserIntegrationMap { get; set; }

        internal static IIntegrationMap<RoleMap> RoleIntegrationMap { get; set; }

        internal static IIntegrationMap<UserRoleMap> UserRoleIntegrationMap { get; set; }

        public static void SetIntegrationMap<T>(IIntegrationMap<T> map)
        {
            if (typeof(T) == typeof(UserMap))
                UserIntegrationMap = (IIntegrationMap<UserMap>) map;

            if (typeof(T) == typeof(RoleMap))
                RoleIntegrationMap = (IIntegrationMap<RoleMap>) map;

            if(typeof(T) == typeof(UserRoleMap))
                UserRoleIntegrationMap = (IIntegrationMap<UserRoleMap>) map;
        }

        public static bool HasIntegrationMap<T>()
        {
            if (typeof(T) == typeof(UserMap))
                return UserIntegrationMap != null;

            if (typeof(T) == typeof(RoleMap))
                return RoleIntegrationMap != null;

            if (typeof(T) == typeof(UserRoleMap))
                return UserRoleIntegrationMap != null;

            return false;

        }
    }
}