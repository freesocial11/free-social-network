using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.CustomFields
{
    public class CustomFieldItemModel : RootModel
    {
        public string FieldName { get; set; }
        
        public string FieldValue { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
    }
}