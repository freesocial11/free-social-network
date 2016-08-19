using System.Collections.Generic;
using mobSocial.Data.Enum;

namespace mobSocial.WebApi.Models.Settings
{
    public class UserSettingsModel
    {
        public UserSettingsModel()
        {
            AvailableUserRegistrationModes = new List<dynamic>();
        }

        /// <summary>
        /// Default registration mode for users
        /// </summary>
        public RegistrationMode UserRegistrationDefaultMode { get; set; }

        /// <summary>
        /// Specifies if user names are enabled for site
        /// </summary>
        public bool AreUserNamesEnabled { get; set; }

        public List<dynamic> AvailableUserRegistrationModes { get; set; }
    }
}