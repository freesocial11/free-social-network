using mobSocial.Data.Entity.Settings;

namespace mobSocial.Data.Extensions
{
    public static class SettingExtensions
    {
        public static bool GetBoolean(this Setting setting)
        {
            return setting.Value.GetBoolean();
        }

       
    }
}