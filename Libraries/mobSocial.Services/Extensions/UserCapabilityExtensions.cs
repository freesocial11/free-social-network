using System.Linq;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Interfaces;
using mobSocial.Services.Users;

namespace mobSocial.Services.Extensions
{
    public static class UserCapabilityExtensions
    {
        /// <summary>
        /// Checks if the user has, listed capabilities.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="capabilityName"></param>
        /// <returns>True if all the provided capabilities are possesed by user. False otherwise.</returns>
        public static bool Can(this User user, params string[] capabilityName)
        {
            if (user == null)
                return false;

            //if the user is administrator, he or she should have all capabilities by default
            if (user.IsAdministrator())
                return true;

            var roleCapabilityService = mobSocialEngine.ActiveEngine.Resolve<IRoleCapabilityService>();
            
            //get user's role ids (only active roles)
            var roleIds = user.UserRoles.Select(x => x.Role).Where(x=>  x.IsActive).Select(x => x.Id);

            //and now the capabilities
            var capabilities = roleCapabilityService.GetConsolidatedCapabilities(roleIds.ToArray());

            return capabilities.Select(x => x.CapabilityName).Intersect(capabilityName).Count() == capabilityName.Length;
        }

        /// <summary>
        /// Checks if the user has either of listed capabilities
        /// </summary>
        /// <param name="user"></param>
        /// <param name="capabilityName"></param>
        /// <returns>True if at least one capability is possessed, false otherwise</returns>
        public static bool CanEither(this User user, params string[] capabilityName)
        {
            if (user == null)
                return false;

            //if the user is administrator, he or she should have all capabilities by default
            if (user.IsAdministrator())
                return true;

            var roleCapabilityService = mobSocialEngine.ActiveEngine.Resolve<IRoleCapabilityService>();

            //get user's role ids (only active roles)
            var roleIds = user.UserRoles.Select(x => x.Role).Where(x => x.IsActive).Select(x => x.Id);

            //and now the capabilities
            var capabilities = roleCapabilityService.GetConsolidatedCapabilities(roleIds.ToArray());

            return capabilities.Select(x => x.CapabilityName).Intersect(capabilityName).Any();
        }

        public static bool IsAdministrator(this User user)
        {
            return user != null && user.Is(SystemRoleNames.Administrator);
        }

        public static bool IsVisitor(this User user)
        {
            return user == null || user.Is(SystemRoleNames.Visitor);
        }

        public static bool IsRegistered(this User user)
        {
            return user != null && (user.Is(SystemRoleNames.Registered) || user.IsAdministrator());
        }
       
        /// <summary>
        /// Checks if the user holds the given role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleSystemName"></param>
        /// <returns></returns>
        public static bool Is(this User user, string roleSystemName)
        {
            return user?.UserRoles != null && user.UserRoles.Any(x => x.Role.SystemName == roleSystemName);
        }

        public static bool CanEditResource<T>(this User user, T resource) where T : IUserResource
        {
            var resourceName = typeof(T).Name;
            var insertCapabilityName = resourceName + "Insert";
            return (user.IsAdministrator()
                || resource.UserId == user.Id/*owner?*/
                || (resource.Id == user.Id && typeof(T) == typeof(User))
                || (resource.Id == 0 && user.Can(insertCapabilityName)) /*new resource?*/); /*agent and has capability to update?*/
        }
    }
}