using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.GroupPages
{
    public class GroupPageMember : BaseEntity
    {
        public virtual int GroupPageId { get; set; }

        public virtual int CustomerId { get; set; }

        public virtual int DisplayOrder { get; set; }


        public virtual GroupPage GroupPage { get; set; }

    }
}