using System;
using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPagePublicModel : RootModel
    {
        public virtual DateTime CreatedOn { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        public UserResponseModel CreatedBy { get; set; }

        public UserResponseModel UpdatedBy { get; set; }

        public virtual string Description { get; set; }

        public virtual string TeamPictureUrl { get; set; }

        public virtual string Name { get; set; }

        public virtual int Id { get; set; }

        public virtual IList<TeamPageGroupPublicModel> Groups { get; set; }

        
    }
}
