using mobSocial.Core.Config;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Settings
{
    public class SecuritySettings : ISettingGroup
    {
        /// <summary>
        /// Default password format
        /// </summary>
        public PasswordFormat DefaultPasswordStorageFormat { get; set; }
    }
}