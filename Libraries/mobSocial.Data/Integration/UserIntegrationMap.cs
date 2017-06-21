using System.Collections.Generic;
using mobSocial.Data.Entity.Users;

namespace mobSocial.Data.Integration
{
    public class UserIntegrationMap : IIntegrationMap<UserMap>
    {
        public string SourceTableName => "mobSocial_User";

        public Dictionary<string, string> SourceToDestinationColumnMapping => new Dictionary<string, string>
        {
            {"Id", "Id"},
            {"FirstName", "FirstName"},
            {"LastName", "LastName"},
            {"Name", "Name"},
            {"Email", "Email"},
            {"UserName", "UserName"},
            {"Guid", "Guid"},
            {"Password", "Password"},
            {"PasswordSalt", "PasswordSalt"},
            {"PasswordFormat", "PasswordFormat"},
            {"Active", "Active"},
            {"DateCreated", "DateCreated"},
            {"DateUpdated", "DateUpdated"},
            {"LastLoginDate", "LastLoginDate"},
            {"IsSystemAccount", "IsSystemAccount"},
            {"Remarks", "Remarks"},
            {"LastLoginIpAddress", "LastLoginIpAddress"},
            {"ReferrerId", "ReferrerId"},
            {"Deleted", "Deleted"}
        };

        public string WhereString { get; }
    }
}