using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;

namespace mobSocial.Services.Users
{
    public interface IUserRegistrationService
    {
        /// <summary>
        /// Tries to register a new user and returns if the registration succeeded or failed
        /// </summary>
        /// <returns></returns>
        UserRegistrationStatus Register(string email, string password, PasswordFormat passwordFormat);

        /// <summary>
        /// Tries to register a new user and returns if the registration succeded or failed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordFormat"></param>
        /// <returns></returns>
        UserRegistrationStatus Register(User user, PasswordFormat passwordFormat);
    }
}