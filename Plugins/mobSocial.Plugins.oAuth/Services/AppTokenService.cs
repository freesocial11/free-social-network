using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Plugins.OAuth.Entity.OAuth;

namespace mobSocial.Plugins.OAuth.Services
{
    public class AppTokenService : BaseEntityService<AppToken>, IAppTokenService
    {
        public AppTokenService(IDataRepository<AppToken> dataRepository) : base(dataRepository)
        {
        }
    }
}