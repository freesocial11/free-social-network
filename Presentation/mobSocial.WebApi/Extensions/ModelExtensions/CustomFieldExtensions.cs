using mobSocial.Data.Entity.CustomFields;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Models.CustomFields;

namespace mobSocial.WebApi.Extensions.ModelExtensions
{
    public static class CustomFieldExtensions
    {
        public static CustomFieldResponseModel ToModel(this CustomField customField, User forUser)
        {
            //are we showing this to a public user or a registered user
            var isRegisteredUser = forUser.IsVisitor() || forUser.IsRegistered();
            var model = new CustomFieldResponseModel()
            {
                DefaultValue = customField.DefaultValue,
                DisplayOrder = customField.DisplayOrder,
                FieldGeneratorMarkup = customField.FieldGeneratorMarkup,
                FieldType = customField.FieldType,
                IsEditable = !isRegisteredUser,
                ParentFieldId = customField.ParentFieldId,
                ParentFieldValue = customField.ParentFieldValue,
                Id = customField.Id,
                Description = customField.Description,
                Required = customField.Required,
                Label = customField.Label,
                MaximumValue = customField.MaximumValue,
                MinimumValue = customField.MinimumValue,
                AvailableValues = customField.AvailableValues
            };
            return model;
        }

        public static CustomFieldModel ToEntityModel(this CustomField customField, string value = null)
        {
            var model = new CustomFieldModel()
            {
                Id = customField.Id,
                Description = customField.Description,
                DefaultValue = customField.DefaultValue,
                DisplayOrder = customField.DisplayOrder,
                FieldGeneratorMarkup = customField.FieldGeneratorMarkup,
                FieldType = customField.FieldType,
                Label = customField.Label,
                ParentFieldId = customField.ParentFieldId,
                ParentFieldValue = customField.ParentFieldValue,
                Required = customField.Required,
                Visible = customField.Visible,
                MaximumValue = customField.MaximumValue,
                MinimumValue = customField.MinimumValue,
                AvailableValues = customField.AvailableValues,
                Value = value
            };
            return model;
        }
    }
}