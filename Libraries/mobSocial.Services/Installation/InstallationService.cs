using System;
using System.IO;
using System.Linq;
using System.Reflection;
using mobSocial.Core;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Initializer;
using mobSocial.Data.Entity.Emails;
using mobSocial.Data.Entity.Notifications;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Migrations;
using mobSocial.Services.Emails;
using mobSocial.Services.Notifications;
using mobSocial.Services.Security;
using mobSocial.Services.Settings;
using mobSocial.Services.Users;

namespace mobSocial.Services.Installation
{
    public class InstallationService : IInstallationService
    {
        public void Install()
        {
            //DatabaseManager.SetDbInitializer(new CreateOrUpdateTables<DatabaseContext>());

            //run the migrator to install the database
            DatabaseManager.IsDatabaseUpdating = true;
            var migrator = new mobSocialDbMigrator(new Data.Migrations.Configuration());
            if (migrator.GetPendingMigrations().Any())
                migrator.Update();

            DatabaseManager.IsDatabaseUpdating = false;

            //any post installation tasks?
            if(PostInstallationTasks.HasPostInstallationTasks())
                PostInstallationTasks.Execute();

            //mark tables installed
            ApplicationHelper.MarkTablesInstalled();
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

            //seed email templates
            SeedEmailTemplates(defaultUserEmail, installDomain);

            //notification emails
            SeedNotificationEvents();

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
                UserRegistrationDefaultMode = RegistrationMode.WithActivationEmail,
                UserLinkTemplate = "<a href='' data-uid='{0}'>{1}</a>"
            });

            //user settings
            settingService.Save(new UrlSettings() {
                ActivationPageUrl = installDomain + "/activate"
            });

            //skill settings
            settingService.Save(new SkillSettings()
            {
                NumberOfUsersPerPageOnSinglePage = 15
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

        private void SeedEmailTemplates(string adminEmail, string installDomain)
        {
            var emailAccountService = mobSocialEngine.ActiveEngine.Resolve<IEmailAccountService>();
            var emailTemplateService = mobSocialEngine.ActiveEngine.Resolve<IEmailTemplateService>();
            var installEmailTemplatesPath =
                ServerHelper.GetLocalPathFromRelativePath("~/App_Data/InstallData/EmailTemplates/");
            //add email account
            var emailAccount = new EmailAccount()
            {
                Email = "support@" + installDomain,
                FromName = "MobSocial",
                Host = "",
                IsDefault = true,
                Port = 485,
                UseDefaultCredentials = true,
                UseSsl = true,
                UserName = "user"
            };
            emailAccountService.Insert(emailAccount);

            var masterTemplate = new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = true,
                Subject = "Master Template",
                TemplateSystemName = EmailTemplateNames.Master,
                TemplateName = "Master",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.Master)
            };
            emailTemplateService.Insert(masterTemplate);

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your account has been created",
                TemplateSystemName = EmailTemplateNames.UserRegisteredMessage,
                TemplateName = "User Registered",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserRegisteredMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "A new user has registered",
                TemplateSystemName = EmailTemplateNames.UserRegisteredMessageToAdmin,
                TemplateName = "User Registered Administrator",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserRegisteredMessageToAdmin),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your account has been activated",
                TemplateSystemName = EmailTemplateNames.UserActivatedMessage,
                TemplateName = "User Activated",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserActivatedMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Activate your account",
                TemplateSystemName = EmailTemplateNames.UserActivationLinkMessage,
                TemplateName = "User Activation Link",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserActivationLinkMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "We have received a password reset request",
                TemplateSystemName = EmailTemplateNames.PasswordRecoveryLinkMessage,
                TemplateName = "Password Recovery Link",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.PasswordRecoveryLinkMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your password has been changed",
                TemplateSystemName = EmailTemplateNames.PasswordChangedMessage,
                TemplateName = "Password Changed",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.PasswordChangedMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your account has been deactivated",
                TemplateSystemName = EmailTemplateNames.UserDeactivatedMessage,
                TemplateName = "User Account Deactivated",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserDeactivatedMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your account has been deactivated",
                TemplateSystemName = EmailTemplateNames.UserDeactivatedMessageToAdmin,
                TemplateName = "User Account Deactivated Administrator",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template =
                    ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserDeactivatedMessageToAdmin),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "Your account has been deleted",
                TemplateSystemName = EmailTemplateNames.UserAccountDeletedMessage,
                TemplateName = "User Account Deleted",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template = ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserAccountDeletedMessage),
                ParentEmailTemplateId = masterTemplate.Id
            });

            emailTemplateService.Insert(new EmailTemplate()
            {
                AdministrationEmail = adminEmail,
                IsMaster = false,
                Subject = "A user account has been deleted",
                TemplateSystemName = EmailTemplateNames.UserAccountDeletedMessageToAdmin,
                TemplateName = "User Account Deleted Administrator",
                IsSystem = true,
                EmailAccountId = emailAccount.Id,
                Template =
                    ReadEmailTemplate(installEmailTemplatesPath, EmailTemplateNames.UserAccountDeletedMessageToAdmin),
                ParentEmailTemplateId = masterTemplate.Id
            });
        }

     
        private void SeedNotificationEvents()
        {
            var notificationEventService = mobSocialEngine.ActiveEngine.Resolve<INotificationEventService>();
            //get all events from notification event class. use reflection for easy insert
            var fieldInfos = typeof(NotificationEventNames).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fi in fieldInfos)
            {
                if (!fi.IsLiteral || fi.IsInitOnly)
                    continue;
                //it's a constant
                var eventName = fi.GetRawConstantValue().ToString();
                notificationEventService.Insert(new NotificationEvent() {
                    EventName = eventName,
                    Enabled = true
                });
            }
        }

        #region Helper
        private string ReadEmailTemplate(string path, string templateName)
        {
            var filePath = path + templateName + ".html";
            return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        }

        /// <summary>
        /// Update the webconfig file with required settings
        /// </summary>
        private void UpdateWebConfig()
        {
            try
            {
                var applicationConfiguration = mobSocialEngine.ActiveEngine.Resolve<IApplicationConfiguration>();
                var cryptographyService = mobSocialEngine.ActiveEngine.Resolve<ICryptographyService>();
                var key = cryptographyService.GetRandomPassword();
                var salt = cryptographyService.GetRandomPassword();

                applicationConfiguration.SetSetting("encryptionKey", key);
                applicationConfiguration.SetSetting("encryptionSalt", salt);
            }
            catch (Exception)
            {
                //an error occured while modifying config file, may be it's write protected or test mode is on?
            }
        }
        #endregion

    }
}