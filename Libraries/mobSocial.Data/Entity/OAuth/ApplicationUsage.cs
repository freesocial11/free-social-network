using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.OAuth
{
    public class ApplicationUsage : BaseEntity
    {
        public int ApplicationId { get; set; }

        public int UsageCount { get; set; }

        public DateTime LastRequested { get; set; }
    }

    public class ApplicationUsageMap : BaseEntityConfiguration<ApplicationUsage>
    {
        
    }

}