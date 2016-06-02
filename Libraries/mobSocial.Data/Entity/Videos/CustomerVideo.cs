using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Videos
{
    public class CustomerVideo : BaseEntity
    {
        public int CustomerVideoAlbumId { get; set; }

        public string VideoUrl { get; set; }

        public string Caption { get; set; }

        public int DisplayOrder { get; set; }

        public int LikeCount { get; set; }
    }


}