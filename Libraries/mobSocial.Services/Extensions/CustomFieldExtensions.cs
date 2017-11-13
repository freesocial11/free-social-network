using System;
using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Entity.CustomFields;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Enum;
using mobSocial.Data.Extensions;
using mobSocial.Data.Interfaces;
using mobSocial.Services.CustomFields;

namespace mobSocial.Services.Extensions
{
    public static class CustomFieldExtensions
    {
        public const string CustomFieldsRequestKey = "e";

        public static string CustomFieldNameFormat = $"{CustomFieldsRequestKey}_{0}";

        public static CustomFieldValidationResult ValidateValueForUser(this CustomField customField, string fieldValue, User user)
        {
            var required = customField.Required;

            if (required && fieldValue.IsNullEmptyOrWhiteSpace())
                return CustomFieldValidationResult.EmptyValueForRequiredField;

            var visible = customField.Visible;

            if (!visible)
                return CustomFieldValidationResult.NonEditableField;

            switch (customField.FieldType)
            {
                case CustomFieldType.Text:
                case CustomFieldType.TextArea:
                    return CustomFieldValidationResult.ValidField;
                case CustomFieldType.Number:
                    if(!fieldValue.IsNumeric())
                        return CustomFieldValidationResult.InvalidValueForFieldType;

                    if (customField.MinimumValue.IsNumeric())
                    {
                        if (customField.MinimumValue.GetInteger(false) > fieldValue.GetInteger(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    if (customField.MaximumValue.IsNumeric())
                    {
                        if (customField.MaximumValue.GetInteger(false) < fieldValue.GetInteger(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    break;
                case CustomFieldType.Email:
                    if(!fieldValue.IsValidEmail())
                        return CustomFieldValidationResult.InvalidValueForFieldType;
                    break;
                case CustomFieldType.DateTime:
                    if (!fieldValue.IsDateTime())
                        return CustomFieldValidationResult.InvalidValueForFieldType;

                    if (customField.MinimumValue.IsDateTime())
                    {
                        if (customField.MinimumValue.GetDateTime(false) > fieldValue.GetDateTime(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    if (customField.MaximumValue.IsDateTime())
                    {
                        if (customField.MaximumValue.GetDateTime(false) < fieldValue.GetDateTime(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    break;
                case CustomFieldType.Color:
                    if(!fieldValue.IsColor())
                        return CustomFieldValidationResult.InvalidValueForFieldType;
                    break;
                case CustomFieldType.Captcha:
                    break;
                case CustomFieldType.Dropdown:
                    break;
                case CustomFieldType.NestedDropdown:
                    break;
                case CustomFieldType.ImageUpload:
                    break;
                case CustomFieldType.FileUpload:
                    break;
            }

            return CustomFieldValidationResult.ValidField;
        }

        public static IList<Tuple<CustomField, string>> GetCustomFields<T>(this IHasEntityProperties<T> entity) where T : BaseEntity
        {
            var entityProperties = entity.GetProperties();
            var customFieldService = mobSocialEngine.ActiveEngine.Resolve<ICustomFieldService>();
            var entityName = typeof(T).Name;
            var typeCustomFields = customFieldService.Get(x => x.EntityName == entityName).ToList();

            var customFieldList = new List<Tuple<CustomField, string>>();

            foreach (var ef in typeCustomFields)
            {
                var fieldName = ef.SystemName;
                var ep = entityProperties.FirstOrDefault(x => x.EntityName == entityName && x.PropertyName == fieldName);
                var fieldValue = ef.DefaultValue;
                if (ep != null)
                {
                    fieldValue = ep.Value;
                }
                customFieldList.Add(new Tuple<CustomField, string>(ef, fieldValue));
            }
            return customFieldList;
        }
    }
}