using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.TeamPages
{
    public class TeamPageModel : RootEntityModel
    {

        public virtual DateTime CreatedOn { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        public virtual int UpdatedBy { get; set; }

        public virtual string Description { get; set; }

        public virtual int TeamPictureId { get; set; }

        public virtual string TeamPictureUrl { get; set; }

        [Required]
        public virtual string Name { get; set; }

        public virtual bool IsEditable { get; set; }

        public virtual IList<TeamPageGroupPublicModel> Groups { get; set; }
    }
}