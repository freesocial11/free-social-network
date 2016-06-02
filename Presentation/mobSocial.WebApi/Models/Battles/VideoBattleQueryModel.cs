using mobSocial.Data.Enum;
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Battles
{
    public class VideoBattleQueryModel: RootModel
    {
        public string ViewType { get; set; }

        public string SearchTerm { get; set; }

        public int UserId { get; set; }

        public BattlesSortBy? BattlesSortBy { get; set; }

        public SortOrder? SortOrder { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

    }
}
