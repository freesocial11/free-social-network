using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Plugins.OAuth.Entity.OAuth;

namespace mobSocial.Plugins.OAuth.Services
{
    public class ClientService : BaseEntityService<OAuthClient>, IClientService
    {
        public ClientService(IDataRepository<OAuthClient> dataRepository) : base(dataRepository)
        {
        }
    }
}