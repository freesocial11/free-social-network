using System.ComponentModel.DataAnnotations;
using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Sponsors
{
    public class SponsorDataModel : RootModel
    {
        public int Id { get; set; }

        public int BattleId { get; set; }

        public BattleType BattleType { get; set; }

        public int SponsorCustomerId { get; set; }

        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        public int DisplayOrder { get; set; }

        public string TargetUrl { get; set; }

        [Required]
        public string DisplayName { get; set; }

        
    }
}