using System;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoParticipantPublicModel : RootModel
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public DateTime LastUpdated { get; set; }

        public string VideoPath { get; set; }

        public string ThumbnailPath { get; set; }

        public string MimeType { get; set; }

        public string ParticipantName { get; set; }

        public string SeName { get; set; }

        public string ParticipantProfileImageUrl { get; set; }

        public int RatingCountLike { get; set; }

        public int RatingCountDislike { get; set; }

        public decimal AverageRating { get; set; }

        public int TotalVoters { get; set; }

        public bool CanEdit { get; set; }

        public bool IsLeading { get; set; }

        public bool IsWinner { get; set; }

        public BattleParticipantStatus VideoBattleParticipantStatus { get; set; }

        public string Remarks { get; set; }
        /// <summary>
        /// Has the logged in user voted for this participant
        /// </summary>
        public VideoBattleVotePublicModel CurrentUserVote { get; set; }

        /// <summary>
        /// Has the logged in user has watched the video of participant
        /// </summary>
        public bool VideoWatched { get; set; }

    }
}
