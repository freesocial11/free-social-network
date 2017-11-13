using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.CustomFields
{
    public class CustomField : BaseEntity, ISoftDeletable
    {
        public string EntityName { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public CustomFieldType FieldType { get; set; }

        public bool Visible { get; set; }

        public bool Required { get; set; }

        public string DefaultValue { get; set; }

        public int? ParentFieldId { get; set; }

        public virtual CustomField ParentField { get; set; }

        public string ParentFieldValue { get; set; }

        public string FieldGeneratorMarkup { get; set; }

        public int DisplayOrder { get; set; }

        public string MinimumValue { get; set; }

        public string MaximumValue { get; set; }

        public string AvailableValues { get; set; }

        public bool Deleted { get; set; }

        public int ApplicationId { get; set; }

        public string SystemName { get; set; }
    }

    public class CustomFieldMap : BaseEntityConfiguration<CustomField> { }
}