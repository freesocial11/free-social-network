using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Data.Entity.Users
{
    public static class SystemRoleNames
    {
        private static readonly IRoleNameProvider RoleNameProvider;
        static SystemRoleNames()
        {
            RoleNameProvider = mobSocialEngine.ActiveEngine.Resolve<IRoleNameProvider>();
        }

        public static string Administrator => RoleNameProvider.Administrator;

        public static string Visitor => RoleNameProvider.Visitor;

        public static string Registered => RoleNameProvider.Registered;
    }
}