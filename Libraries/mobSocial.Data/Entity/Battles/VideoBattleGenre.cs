using mobSocial.Core.Data;
using mobSocial.Data.Entity.Videos;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattleGenre : BaseEntity
    {
        public int VideoBattleId { get; set; }

        public int VideoGenreId { get; set; }

        public virtual VideoBattle VideoBattle { get; set; }

        public virtual VideoGenre VideoGenre { get; set; }
    }

    public class VideoBattleGenreMap : BaseEntityConfiguration<VideoBattleGenre> { }
}
