using System.Linq;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Enum;
using mobSocial.Data.Extensions;
using mobSocial.Services.CustomFields;
using mobSocial.Services.Extensions;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Models.CustomFields;

namespace mobSocial.WebApi.Configuration.Mvc.Models
{
    public class WithCustomFieldsModel<TModelType> : RootCustomFieldModel where TModelType : RootModel
    {
        public TModelType Model { get; set; }

        public bool ValidateCustomFieldsForEntity<T>(int applicationId) where T : BaseEntity
        {
            var customFieldService = mobSocialEngine.ActiveEngine.Resolve<ICustomFieldService>();
            var entityName = typeof(T).Name;

            var dbCustomFields = customFieldService.Get(x => x.EntityName == entityName && x.ApplicationId == applicationId).ToList();

            //current user is registered user?
            var currentUser = ApplicationContext.Current.CurrentUser;

            //remove all the custom fields which are non existant
            var dbCustomFieldNames = dbCustomFields.Select(x => x.SystemName);
            SubmittedCustomFields = SubmittedCustomFields.Where(x => dbCustomFieldNames.Contains(x.FieldName)).ToList();

            foreach (var dbExField in dbCustomFields)
            {
                var displayableFieldLabel = dbExField.Label;

                var expectedFieldName = dbExField.SystemName;

                var sExField = SubmittedCustomFields.FirstOrDefault(x => x.FieldName == expectedFieldName);
                if (sExField == null)
                {
                   
                    //#1 if user is required to submit this field
                    if (dbExField.Required)
                    {
                        SubmittedCustomFields.Add(new CustomFieldItemModel()
                        {
                            FieldName = expectedFieldName,
                            FieldValue = null,
                            IsValid = false,
                            ValidationMessage = $"Field '{displayableFieldLabel}' is required"
                        });
                    }
                }
                else
                {
                    //if field was submitted, let's validate if valid values where submitted
                    var validationResult = dbExField.ValidateValueForUser(sExField.FieldValue, currentUser);
                    if (validationResult != CustomFieldValidationResult.ValidField)
                    {
                        sExField.IsValid = false;
                        switch (validationResult)
                        {
                            case CustomFieldValidationResult.InvalidValueForFieldType:
                                sExField.ValidationMessage = $"Invalid value for '{displayableFieldLabel}' submitted";
                                break;
                            case CustomFieldValidationResult.EmptyValueForRequiredField:
                                sExField.ValidationMessage = $"Field '{displayableFieldLabel}' is required";
                                break;
                            case CustomFieldValidationResult.OutOfRangeValue:
                                sExField.ValidationMessage = $"Invalid value for '{displayableFieldLabel}' submitted";
                                break;
                            case CustomFieldValidationResult.NonEditableField:
                                sExField.ValidationMessage = $"'{displayableFieldLabel}' can't be edited";
                                break;
                        }
                    }
                    else
                    {
                        //we can set value to a default one if empty
                        if (sExField.FieldValue.IsNullEmptyOrWhiteSpace())
                        {
                            sExField.FieldValue = dbExField.DefaultValue;
                        }
                        sExField.IsValid = true;
                    }
                        
                }
            }
            return SubmittedCustomFields.All(x => x.IsValid);
        }
        
    }
}