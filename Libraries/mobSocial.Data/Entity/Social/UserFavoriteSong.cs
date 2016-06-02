using mobSocial.Core.Data;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Social
{
    public class UserFavoriteSong : BaseEntity, ISoftDeletable
    {
        public UserFavoriteSong()
        {
            IsDeleted = false;
            DisplayOrder = 0;
        }

        public int UserId { get; set; }
        public int TrackId { get; set; }
        public string Title { get; set; }
        public string PreviewUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDeleted { get; set; }

        public bool Deleted { get; set; }
    }

}




