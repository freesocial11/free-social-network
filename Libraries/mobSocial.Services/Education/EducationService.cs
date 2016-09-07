using mobSocial.Core.Data;
using mobSocial.Core.Services;

namespace mobSocial.Services.Education
{
    public class EducationService : BaseEntityService<Data.Entity.Education.Education>, IEducationService
    {
        public EducationService(IDataRepository<Data.Entity.Education.Education> dataRepository) : base(dataRepository)
        {
        }
    }
}
