namespace mobSocial.Data.Entity.Users
{
    public interface IRoleNameProvider
    {
        string Administrator { get; }

        string Visitor { get; }

        string Registered { get; }
    }
}
