using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Initializer;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Migrations;
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

        public void FillRequiredSeedData(string defaultUserEmail, string defaultUserPassword)
        {
            //first the settings
            SeedSettings();
            
            //seed the roles
            SeedRoles();

            //then the user
            SeedDefaultUser(defaultUserEmail, defaultUserPassword);
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

            roleService.Insert(new Role() {
                RoleName = SystemRoleNames.Agent,
                IsSystemRole = false,
                IsActive = true,
                SystemName = SystemRoleNames.Agent
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
        private void SeedSettings()
        {
            var settingService = mobSocialEngine.ActiveEngine.Resolve<ISettingService>();

            //general settings
            settingService.Save(new GeneralSettings()
            {
                
            });

            //media settings
            settingService.Save(new MediaSettings() {

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

    }
}