using mobSocial.Core.Data;

namespace mobSocial.Data.Extensions
{
    public static class EntityExtensions
    {
        public static string GetUnproxiedTypeName(this object entity)
        {
            var type = entity.GetType();
            if (type.Namespace == "System.Data.Entity.DynamicProxies")
            {
                type = type.BaseType;
            }
            return type?.Name; //check for proxy types
        }
    }
}