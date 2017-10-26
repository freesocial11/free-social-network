using System.Collections.Generic;
using mobSocial.WebApi.Models.CustomFields;

namespace mobSocial.WebApi.Configuration.Mvc.Models
{
    public abstract class RootCustomFieldModel : RootModel
    {
        public IList<CustomFieldItemModel> SubmittedCustomFields { get; set; }
    }
}