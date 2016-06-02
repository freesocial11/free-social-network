using System.Collections.Generic;
using System.Web.Mvc;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Payment
{
    public class UserPaymentPublicModel : RootModel
    {
        public UserPaymentPublicModel()
        {
            UserPaymentMethods = new List<SelectListItem>();
        }

        public IList<SelectListItem> UserPaymentMethods { get; set; }

        public decimal AvailableCredits { get; set; }

        public decimal UsableCredits { get; set; }

        public decimal MinimumPaymentAmount { get; set; }

        public bool IsAmountVariable { get; set; }

        public PurchaseType PurchaseType { get; set; }

    }
}