using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Battles
{
    public class VideoBattleVideo : BaseEntity
    {
        public string VideoPath { get; set; }

        public string MimeType { get; set; }

        public int ParticipantId { get; set; }

        public int VideoBattleId { get; set; }

        public VideoStatus VideoStatus { get; set; }

        public virtual VideoBattle VideoBattle { get; set; }

        public string ThumbnailPath { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }

    public class VideoBattleVideoMap: BaseEntityConfiguration<VideoBattleVideo> { }
}
