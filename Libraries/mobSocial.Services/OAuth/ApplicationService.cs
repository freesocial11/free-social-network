using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Services.OAuth
{
    public class ApplicationService : BaseEntityService<OAuthApplication>, IApplicationService
    {
        public ApplicationService(IDataRepository<OAuthApplication> dataRepository) : base(dataRepository)
        {
        }
    }
}