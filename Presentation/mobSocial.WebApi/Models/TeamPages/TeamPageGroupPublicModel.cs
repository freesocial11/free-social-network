using System;
using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPageGroupPublicModel : RootModel
    {
        public int Id { get; set; }

        public int TeamPageId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public string PaypalDonateUrl { get; set; }

        public IList<UserResponseModel> GroupMembers { get; set; }
    }
}