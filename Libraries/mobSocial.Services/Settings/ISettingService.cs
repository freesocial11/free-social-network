using mobSocial.Core.Config;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Settings;

namespace mobSocial.Services.Settings
{
    public interface ISettingService : IBaseEntityService<Setting>
    {
        Setting Get<T>(string keyName) where T : ISettingGroup;

        void Save<T>(string keyName, string keyValue) where T : ISettingGroup;

        void Save<T>(T settings) where T: ISettingGroup;

        T GetSettings<T>() where T : ISettingGroup;

        void LoadSettings<T>(T settingsObject) where T : ISettingGroup;
    }
}