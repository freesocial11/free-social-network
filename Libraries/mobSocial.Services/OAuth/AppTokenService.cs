using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Services.OAuth
{
    public class AppTokenService : BaseEntityService<AppToken>, IAppTokenService
    {
        public AppTokenService(IDataRepository<AppToken> dataRepository) : base(dataRepository)
        {
        }
    }
}