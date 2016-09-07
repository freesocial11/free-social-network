using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Education;

namespace mobSocial.Services.Education
{
    public class SchoolService : BaseEntityService<School>, ISchoolService
    {
        public SchoolService(IDataRepository<School> dataRepository) : base(dataRepository)
        {
        }
    }
}