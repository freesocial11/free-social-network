using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.CustomFields;

namespace mobSocial.Services.CustomFields
{
    public class CustomFieldService : BaseEntityService<CustomField>, ICustomFieldService
    {
        public CustomFieldService(IDataRepository<CustomField> dataRepository) : base(dataRepository) { }
    }
}