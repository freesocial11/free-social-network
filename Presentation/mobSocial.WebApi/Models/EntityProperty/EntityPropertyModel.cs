using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.EntityProperty
{
    public class EntityPropertyModel : RootModel
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string PropertyName { get; set; }

        public string Value { get; set; }
    }
}