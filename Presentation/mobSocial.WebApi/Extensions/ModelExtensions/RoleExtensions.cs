using mobSocial.Data.Entity.Users;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class RoleExtensions
    {
        public static RoleResponseModel ToModel(this Role role)
        {
            var model = new RoleResponseModel()
            {
                Id = role.Id,
                IsActive = role.IsActive,
                IsSystemRole = role.IsSystemRole,
                RoleName = role.RoleName,
                SystemName = role.SystemName
            };
            return model;
        }
    }
}