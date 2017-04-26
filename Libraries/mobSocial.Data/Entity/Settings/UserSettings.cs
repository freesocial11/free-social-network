using mobSocial.Core.Config;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Settings
{
    public class UserSettings: ISettingGroup
    {
        /// <summary>
        /// Default registration mode for users
        /// </summary>
        public RegistrationMode UserRegistrationDefaultMode { get; set; }

        /// <summary>
        /// Specifies if user names are enabled for site
        /// </summary>
        public bool AreUserNamesEnabled { get; set; }

        /// <summary>
        /// Specifies if an email is also required to activate the user along with activation code
        /// </summary>
        public bool RequireEmailForUserActivation { get; set; }

        /// <summary>
        /// Specifies the html template used for creating user links within content
        /// </summary>
        public string UserLinkTemplate { get; set; }

        /// <summary>
        /// Specifies minimum length of string required to search users #imported from old webapi plugin
        /// </summary>
        public int PeopleSearchTermMinimumLength { get; set; }

    }
}