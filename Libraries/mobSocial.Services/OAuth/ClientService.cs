using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Services.OAuth
{
    public class ClientService : BaseEntityService<OAuthClient>, IClientService
    {
        public ClientService(IDataRepository<OAuthClient> dataRepository) : base(dataRepository)
        {
        }
    }
}