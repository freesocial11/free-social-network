using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Authentication
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Signs in the user
        /// </summary>
        LoginStatus SignIn(string email, bool isPersistent = false,  bool forceCreateNewAccount = false);


        /// <summary>
        /// Creates an authentication ticket for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        void CreateAuthenticationTicket(User user, bool isPersistent = false);

        /// <summary>
        /// Clears the authentication ticket of the user
        /// </summary>
        /// <param name="user"></param>
        void ClearAuthenticationTicket();

        /// <summary>
        /// Gets the current user
        /// </summary>
        /// <returns></returns>
        User GetCurrentUser();
    }
}