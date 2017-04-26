using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.TeamPages;

namespace mobSocial.Data.Entity.GroupPages
{

    public class GroupPage : BaseEntity
    {

        public GroupPage()
        {
            Members = new List<GroupPageMember>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string PayPalDonateUrl { get; set; }

        public virtual List<GroupPageMember> Members { get; set; }

        public virtual TeamPage Team { get; set; }

        public int TeamPageId { get; set; }

        /// <summary>
        /// Display order of this group on the Team Page
        /// </summary>
        public int DisplayOrder { get; set; }

        public bool IsDefault { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

    }

    public class GroupPageMap : BaseEntityConfiguration<GroupPage> { }

}



