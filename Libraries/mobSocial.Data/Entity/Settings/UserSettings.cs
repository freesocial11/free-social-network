using mobSocial.Core.Config;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Settings
{
    public class UserSettings: ISettingGroup
    {
        public RegistrationMode UserRegistrationDefaultMode { get; set; }
    }
}