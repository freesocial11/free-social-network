using System.Collections.Generic;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Integration
{
    public class RoleIntegrationMap : IIntegrationMap<RoleMap>
    {
        public string SourceTableName => "mobSocial_Role";

        public Dictionary<string, string> SourceToDestinationColumnMapping => new Dictionary<string, string>()
        {
            {"Id", "Id"},
            {"RoleName", "RoleName"},
            {"IsSystemRole", "IsSystemRole" },
            {"SystemName", "SystemName" },
            {"IsActive", "IsActive" },
        };

        public string WhereString { get; set; }
    }
}