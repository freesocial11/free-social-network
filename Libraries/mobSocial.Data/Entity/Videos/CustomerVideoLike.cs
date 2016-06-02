using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Videos
{
    public class CustomerVideoLike : BaseEntity
    {
        public int CustomerId { get; set; }
        public int CustomerVideoId { get; set; }

        
    }


}