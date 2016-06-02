using System;
using System.Linq;
using mobSocial.Core.Config;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.Utils;
using mobSocial.Data.Entity.Settings;

namespace mobSocial.Services.Settings
{
    public class SettingService : MobSocialEntityService<Setting>, ISettingService
    {
        public SettingService(IDataRepository<Setting> dataRepository)
            : base(dataRepository)
        {
        }

        public Setting Get<T>(string keyName) where T : ISettingGroup
        {
            var groupName = typeof(T).Name;
            var settings = Repository.Get(x => x.Key == keyName);
            if (!string.IsNullOrEmpty(groupName))
                settings = settings.Where(x => x.GroupName == groupName);

            return settings.FirstOrDefault();
        }

        public void Save<T>(string keyName, string keyValue) where T : ISettingGroup
        {
            var groupName = typeof(T).Name;

            //check if setting exist
            var setting = Get<T>(keyName);
            if (setting == null)
            {
                setting = new Setting() {
                    GroupName = groupName,
                    Key = keyName,
                    Value = keyValue
                };
                Repository.Insert(setting);
            }
            else
            {
                setting.Value = keyValue;
                Repository.Update(setting);
            }
        }

        public void Save<T>(T settings) where T : ISettingGroup
        {
            //each setting group will have some properties. We'll loop through these using reflection
            var propertyFields = typeof(T).GetProperties();
            foreach (var property in propertyFields)
            {
                var propertyName = property.Name;
                var valueObj = property.GetValue(settings);
                var value = valueObj == null ? "" : valueObj.ToString();
                //save the property
                Save<T>(propertyName, value);
            }
        }

        public T GetSettings<T>() where T : ISettingGroup
        {
            //create a new settings object
            var settingsObj = Activator.CreateInstance<T>();

            FurnishInstance(settingsObj);

            return settingsObj;
        }

        public void LoadSettings<T>(T settingsObject) where T : ISettingGroup
        {
            FurnishInstance(settingsObject);
        }

        private void FurnishInstance<T>(T settingsInstance) where T : ISettingGroup
        {
            var settingInstanceType = settingsInstance.GetType();
            //each setting group will have some properties. We'll loop through these using reflection
            var propertyFields = settingInstanceType.GetProperties();

            foreach (var property in propertyFields)
            {
                var propertyName = property.Name;

                //retrive the value of setting from db
                var savedSettingEntity =
                    Get(x => x.Key == propertyName && x.GroupName == settingInstanceType.Name, null).FirstOrDefault();

                if (savedSettingEntity != null)
                    //set the property
                    property.SetValue(settingsInstance, TypeConverter.CastPropertyValue(property, savedSettingEntity.Value));
            }
        }
    }
}