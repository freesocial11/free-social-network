using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.BusinessPages
{
    public class BusinessPageCoupon : BaseEntity
    {
        public int BusinessPageId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string BriefDescription { get; set; }
        public string Disclaimer { get; set; }
        public int UsageCount { get; set; }
        public int DisplayOrder { get; set; }


        public virtual BusinessPage BusinessPage { get; set; }
    }
}