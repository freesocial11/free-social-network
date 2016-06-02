using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.EntityProperties;
using mobSocial.Data.Interfaces;
using mobSocial.Services.EntityProperties;

namespace mobSocial.Services.Extensions
{
    public static class EntityPropertyExtensions
    {
        /// <summary>
        /// Gets the properties of entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IList<EntityProperty> GetProperties<T>(this IHasEntityProperties<T> entity) where T: BaseEntity
        {
            var entityPropertyService = mobSocialEngine.ActiveEngine.Resolve<IEntityPropertyService>();
            return entityPropertyService.Get(x => x.EntityName == typeof(T).Name && x.EntityId == entity.Id, null).ToList();
        }
        /// <summary>
        /// Gets the property with specified name for current entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static EntityProperty GetProperty<T>(this IHasEntityProperties<T> entity, string propertyName) where T : BaseEntity
        {
            var entityPropertyService = mobSocialEngine.ActiveEngine.Resolve<IEntityPropertyService>();
            return
                entityPropertyService.Get(
                    x => x.EntityName == typeof(T).Name && x.EntityId == entity.Id && x.PropertyName == propertyName,
                    null).FirstOrDefault();
        }

        /// <summary>
        /// Gets the property valueas stored
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this IHasEntityProperties<T> entity, string propertyName) where T : BaseEntity
        {
            var entityProperty = GetProperty(entity, propertyName);
            return entityProperty?.Value;
        }

        /// <summary>
        /// Gets property value as the target type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetPropertyValueAs<T>(this IHasEntityProperties entity, string propertyName, T defaultValue = default(T))
        {
            var entityPropertyService = mobSocialEngine.ActiveEngine.Resolve<IEntityPropertyService>();
            var typeName = entity.GetType().Name;
            var entityProperty =  entityPropertyService.Get(
                    x => x.EntityName == typeName && x.EntityId == entity.Id && x.PropertyName == propertyName,
                    null).FirstOrDefault();

            if (entityProperty == null)
                return defaultValue;

            return (T) entityProperty.Value;
        }

        public static void SetPropertyValue<T>(this IHasEntityProperties<T> entity, string propertyName, object value)
            where T : BaseEntity
        {
            //does this property exist?
            var property = GetProperty(entity, propertyName) ?? new EntityProperty()
            {
                EntityId = entity.Id,
                EntityName = typeof(T).Name,
                PropertyName = propertyName
            };
            

            property.Value = value;
            var entityPropertyService = mobSocialEngine.ActiveEngine.Resolve<IEntityPropertyService>();
            if (property.Id == 0)
                entityPropertyService.Insert(property);
            else
                entityPropertyService.Update(property);
        }
    }
}