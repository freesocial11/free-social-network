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

        public static CustomFieldValidationResult ValidateValueForUser(this CustomField extraField, string fieldValue, User user)
        {
            var required = extraField.Required;

            if (required && fieldValue.IsNullEmptyOrWhiteSpace())
                return CustomFieldValidationResult.EmptyValueForRequiredField;

            var visible = extraField.Visible;

            if (!visible)
                return CustomFieldValidationResult.NonEditableField;

            switch (extraField.FieldType)
            {
                case CustomFieldType.Number:
                    if(!fieldValue.IsNumeric())
                        return CustomFieldValidationResult.InvalidValueForFieldType;

                    if (extraField.MinimumValue.IsNumeric())
                    {
                        if (extraField.MinimumValue.GetInteger(false) > fieldValue.GetInteger(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    if (extraField.MaximumValue.IsNumeric())
                    {
                        if (extraField.MaximumValue.GetInteger(false) < fieldValue.GetInteger(false))
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

                    if (extraField.MinimumValue.IsDateTime())
                    {
                        if (extraField.MinimumValue.GetDateTime(false) > fieldValue.GetDateTime(false))
                        {
                            return CustomFieldValidationResult.OutOfRangeValue;
                        }
                    }
                    if (extraField.MaximumValue.IsDateTime())
                    {
                        if (extraField.MaximumValue.GetDateTime(false) < fieldValue.GetDateTime(false))
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
                default:
                    return CustomFieldValidationResult.UnknownError;
            }

            return CustomFieldValidationResult.ValidField;
        }

        public static string GetDbFieldName(this CustomField extraField)
        {
            return string.Format(CustomFieldNameFormat, extraField.Id);
        }

        public static IList<Tuple<CustomField, string>> GetCustomFields<T>(this IHasEntityProperties<T> entity) where T : BaseEntity
        {
            var entityProperties = entity.GetProperties();
            var extraFieldService = mobSocialEngine.ActiveEngine.Resolve<ICustomFieldService>();
            var entityName = typeof(T).Name;
            var typeCustomFields = extraFieldService.Get(x => x.EntityName == entityName).ToList();

            var extraFieldList = new List<Tuple<CustomField, string>>();

            foreach (var ef in typeCustomFields) {
                var fieldName = ef.GetDbFieldName();
                var ep = entityProperties.FirstOrDefault(x => x.EntityName == entityName && x.PropertyName == fieldName);
                var fieldValue = ef.DefaultValue;
                if (ep != null)
                {
                    fieldValue = ep.Value;
                }
                extraFieldList.Add(new Tuple<CustomField, string>(ef, fieldValue));
            }
            return extraFieldList;
        }
    }
}