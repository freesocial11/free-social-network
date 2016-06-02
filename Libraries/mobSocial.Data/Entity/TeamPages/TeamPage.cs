using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.GroupPages;

namespace mobSocial.Data.Entity.TeamPages
{
    public class TeamPage : BaseEntity
    {

        public TeamPage()
        {
            GroupPages = new List<GroupPage>();
            
        }

        public virtual List<GroupPage> GroupPages { get; set; }

        public virtual DateTime CreatedOn { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime UpdatedOn { get; set; }
        public virtual int UpdatedBy { get; set; }

        public virtual string Description { get; set; }

        public virtual string TeamPictureUrl { get; set; }

        public virtual string Name { get; set; }


    }
}