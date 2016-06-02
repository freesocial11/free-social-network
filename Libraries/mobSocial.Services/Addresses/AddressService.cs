using mobSocial.Core.Data;
using mobSocial.Data.Entity.Addresses;

namespace mobSocial.Services.Addresses
{
    public class AddressService: MobSocialEntityService<Address>, IAddressService
    {
        public AddressService(IDataRepository<Address> dataRepository) : base(dataRepository)
        {
        }
    }
}