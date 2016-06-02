using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Albums
{
    public class CustomerAlbumPicture : BaseEntity
    {
        public int CustomerAlbumId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Caption { get; set; }
        public int DisplayOrder { get; set; }
        public int LikeCount { get; set; }

    }


}