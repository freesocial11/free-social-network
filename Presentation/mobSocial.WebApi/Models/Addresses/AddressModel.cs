using System;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Addresses
{
    public class AddressModel : RootEntityModel
    {
        public int PageOwnerId { get; set; }

        public string Name { get; set; }

        public string RemoteEntityId { get; set; }

        public string RemoteSourceName { get; set; }

        public string Biography { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string HomeTown { get; set; }

        public string ShortDescription { get; set; }
    }
}