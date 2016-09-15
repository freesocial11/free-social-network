using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Initializer;
using mobSocial.Data.Entity.Emails;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Migrations;
using mobSocial.Services.Emails;
using mobSocial.Services.Security;
using mobSocial.Services.Settings;
using mobSocial.Services.Users;

namespace mobSocial.Services.Installation
{
    public class InstallationService : IInstallationService
    {
        public void Install()
        {
            DatabaseManager.SetDbInitializer(new CreateOrUpdateTables<DatabaseContext>());

            //run the migrator to install the database
            var migrator = new mobSocialDbMigrator(new Data.Migrations.Configuration());
            migrator.Update();
        }

        public void Install(string connectionString, string providerName)
        {
            DatabaseManager.SetDbInitializer(new CreateOrUpdateTables<DatabaseContext>());

            //run the migrator to install the database
            var migrator = new mobSocialDbMigrator(new Data.Migrations.Configuration(connectionString, providerName));
            migrator.Update();
        }

        public void FillRequiredSeedData(string defaultUserEmail, string defaultUserPassword, string installDomain)
        {
            //first the settings
            SeedSettings(installDomain);
            
            //seed the roles
            SeedRoles();

            //then the user
            SeedDefaultUser(defaultUserEmail, defaultUserPassword);

            //seed email account
            SeedEmailAccount(installDomain);

            //update config file
            UpdateWebConfig();
        }
        /// <summary>
        /// Seed roles
        /// </summary>
        private void SeedRoles()
        {
            var roleService = mobSocialEngine.ActiveEngine.Resolve<IRoleService>();

            roleService.Insert(new Role()
            {
                RoleName = SystemRoleNames.Administrator,
                IsSystemRole = true,
                IsActive = true,
                SystemName = SystemRoleNames.Administrator
            });

            roleService.Insert(new Role() {
                RoleName = SystemRoleNames.Registered,
                IsSystemRole = true,
                IsActive = true,
                SystemName = SystemRoleNames.Registered
            });

            roleService.Insert(new Role() {
                RoleName = SystemRoleNames.Visitor,
                IsSystemRole = true,
                IsActive = true,
                SystemName = SystemRoleNames.Visitor
            });
            
        }

        /// <summary>
        /// Seed default user
        /// </summary>
        private void SeedDefaultUser(string email, string password)
        {
            var userRegistrationService = mobSocialEngine.ActiveEngine.Resolve<IUserRegistrationService>();
            var securitySettings = mobSocialEngine.ActiveEngine.Resolve<SecuritySettings>();

            var registrationResult = userRegistrationService.Register(email, password, securitySettings.DefaultPasswordStorageFormat);
            if (registrationResult == UserRegistrationStatus.Success)
            {
                //add roles
                var roleService = mobSocialEngine.ActiveEngine.Resolve<IRoleService>();
                var userService = mobSocialEngine.ActiveEngine.Resolve<IUserService>();

                //first get user entity and assign administrator role
                var user = userService.FirstOrDefault(x => x.Email == email);
                if(user != null)
                    roleService.AssignRoleToUser(SystemRoleNames.Administrator, user);

            }
            else
            {
                throw new mobSocialException("Installation failed");
            }
        }

        /// <summary>
        /// Seed settings
        /// </summary>
        private void SeedSettings(string installDomain)
        {
            var settingService = mobSocialEngine.ActiveEngine.Resolve<ISettingService>();

            //general settings
            settingService.Save(new GeneralSettings()
            {
                ImageServerDomain = string.Concat(installDomain, "/api"),
                VideoServerDomain = string.Concat(installDomain, "/api"),
                ApplicationApiRoot = string.Concat(installDomain, "/api"),
                ApplicationUiDomain = installDomain,
                ApplicationCookieDomain = installDomain
            });

            //media settings
            settingService.Save(new MediaSettings() {
                ThumbnailPictureSize = "100x100",
                SmallProfilePictureSize = "64x64",
                MediumProfilePictureSize = "128x128",
                SmallCoverPictureSize = "300x50",
                MediumCoverPictureSize = "800x300",
                PictureSaveLocation = MediaSaveLocation.FileSystem,
                PictureSavePath = "~/Content/Media/Uploads/Images",
                VideoSavePath = "~/Content/Media/Uploads/Videos",
                OtherMediaSaveLocation = MediaSaveLocation.FileSystem,
                OtherMediaSavePath = "~/Content/Media/Uploads/Others",
                DefaultUserProfileImageUrl = "/api/Content/Media/d_male.jpg"
            });

            //system settings
            settingService.Save(new SystemSettings() {
                IsInstalled = true
            });

            //security settings
            settingService.Save(new SecuritySettings() {
                DefaultPasswordStorageFormat = PasswordFormat.Sha1Hashed
            });

            //user settings
            settingService.Save(new UserSettings() {
                UserRegistrationDefaultMode = RegistrationMode.WithActivationEmail
            });
        }

        /// <summary>
        /// Seed email accounts
        /// </summary>
        private void SeedEmailAccount(string installDomain)
        {
            var emailAccountService = mobSocialEngine.ActiveEngine.Resolve<IEmailAccountService>();
            emailAccountService.Insert(new EmailAccount()
            {
                Email = "mailer@" + installDomain,
                FromName = "MobSocial Network",
                Host = "smtp." + installDomain,
                IsDefault = true,
                UseDefaultCredentials = false,
                UseSsl = true,
                UserName = "mailer@" + installDomain,
                Password = "password"
            });
        }

        /// <summary>
        /// Update the webconfig file with required settings
        /// </summary>
        private void UpdateWebConfig()
        {
            var applicationConfiguration = mobSocialEngine.ActiveEngine.Resolve<IApplicationConfiguration>();
            var cryptographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
            var key = cryptographyService.GetRandomPassword();
            var salt = cryptographyService.GetRandomPassword();

            applicationConfiguration.SetSetting("encryptionKey", key);
            applicationConfiguration.SetSetting("encryptionSalt", salt);
        }
    }
}