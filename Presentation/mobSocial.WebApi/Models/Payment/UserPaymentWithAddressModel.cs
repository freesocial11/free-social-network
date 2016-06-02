using mobSocial.WebApi.Models.Payment;

namespace Nop.Plugin.WebApi.MobSocial.Models
{
    public class CustomerPaymentWithAddressModel
    {
        public UserPaymentModel CustomerPaymentModel { get; set; }

        public CustomerAddressEditModel CustomerAddressEditModel { get; set; }
    }
}