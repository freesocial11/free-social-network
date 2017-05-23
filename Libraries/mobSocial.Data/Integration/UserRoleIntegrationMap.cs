using System.Collections.Generic;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Integration
{
    public class UserRoleIntegrationMap : IIntegrationMap<UserRoleMap>
    {
        public string SourceTableName => "mobSocial_UserRole";

        public Dictionary<string, string> SourceToDestinationColumnMapping => new Dictionary<string, string>()
        {
            {"Id", "Id" },
            {"UserId", "UserId"},
            {"RoleId", "RoleId"}
        };
        public string WhereString { get; set; }
    }
}