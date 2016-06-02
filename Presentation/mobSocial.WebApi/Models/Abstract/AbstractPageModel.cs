using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Addresses;

namespace mobSocial.WebApi.Models.Abstract
{
    public abstract class AbstractPageModel : RootEntityModel
    {
        public IList<AddressModel> Addresses { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }
}