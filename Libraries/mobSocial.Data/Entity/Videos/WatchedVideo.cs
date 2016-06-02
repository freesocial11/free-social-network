using System;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Videos
{
    public class WatchedVideo: BaseEntity
    {
        public int VideoId { get; set; }

        public int CustomerId { get; set; }

        public VideoType VideoType { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}