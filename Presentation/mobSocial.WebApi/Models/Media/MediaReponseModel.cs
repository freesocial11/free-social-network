using System;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Models.Media
{
    public class MediaReponseModel : RootModel
    {
        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public MediaType MediaType { get; set; }

        public int Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateCreatedLocal { get; set; }

        public UserResponseModel CreatedBy { get; set; }

        public int TotalLikes { get; set; }

        public int TotalComments { get; set; }

        public bool CanComment { get; set; }

        public int LikeStatus { get; set; }

        public int NextMediaId { get; set; }

        public int PreviousMediaId { get; set; }

        public bool FullyLoaded { get; set; }

    }
}