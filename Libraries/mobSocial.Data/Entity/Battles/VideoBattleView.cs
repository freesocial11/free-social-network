using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattleView : BaseEntity
    {
        public int UserId { get; set; }

        public int VideoBattleVideoId { get; set; }
    }

    public class VideoBattleViewMap: BaseEntityConfiguration<VideoBattleView> { }
}
