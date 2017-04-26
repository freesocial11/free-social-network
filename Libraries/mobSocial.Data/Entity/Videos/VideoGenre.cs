using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Videos
{
    public class VideoGenre: BaseEntity
    {
        public string GenreName { get; set; }
    }

    public class VideoGenreMap : BaseEntityConfiguration<VideoGenre> { }
}
