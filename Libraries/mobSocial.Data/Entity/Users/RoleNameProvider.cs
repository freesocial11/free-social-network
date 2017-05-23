namespace mobSocial.Data.Entity.Users
{
    public class RoleNameProvider : IRoleNameProvider
    {
        public string Administrator => "Administrator";
        public string Visitor => "Visitor";
        public string Registered => "Registered";
    }
}