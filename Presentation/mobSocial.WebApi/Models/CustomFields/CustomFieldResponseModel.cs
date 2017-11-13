using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.CustomFields
{
    public class CustomFieldResponseModel : RootEntityModel
    {
        public string Label { get; set; }

        public CustomFieldType FieldType { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool IsEditable { get; set; }

        public string DefaultValue { get; set; }

        public int? ParentFieldId { get; set; }

        public string ParentFieldValue { get; set; }

        public string FieldGeneratorMarkup { get; set; }

        public int DisplayOrder { get; set; }

        public string MinimumValue { get; set; }

        public string MaximumValue { get; set; }

        public string AvailableValues { get; set; }

        public string Value { get; set; }

        public string SystemName { get; set; }
    }
}