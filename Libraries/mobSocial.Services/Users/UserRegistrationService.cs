using System;
using mobSocial.Core;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Services.Security;

namespace mobSocial.Services.Users
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserService _userService;
        private readonly ICryptographyService _cryptographyService;

        public UserRegistrationService(IUserService userService, 
            ICryptographyService cryptographyService)
        {
            _userService = userService;
            _cryptographyService = cryptographyService;
        }

        public UserRegistrationStatus Register(string email, string password, PasswordFormat passwordFormat)
        {
            //before registering the user, we need to check a few things

            //does the user exist already?
            var existingUser = _userService.FirstOrDefault(x => x.Email == email);
            if (existingUser != null)
                return UserRegistrationStatus.FailedAsEmailAlreadyExists;

            //we can create a user now, we'll need to hash the password
            var salt = _cryptographyService.CreateSalt(8); //64 bits...should be good enough

            var hashedPassword = _cryptographyService.GetHashedPassword(password, salt, passwordFormat);

            //create a new user entity
            var user = new User()
            {
                Email = email,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                LastLoginIpAddress = WebHelper.GetClientIpAddress(),
                Password = hashedPassword,
                PasswordSalt = salt,
                PasswordFormat = passwordFormat,
                Guid = Guid.NewGuid(),
                IsSystemAccount = false,
                Active = true
            };

            _userService.Insert(user);
            return UserRegistrationStatus.Success;
        }
    }
}