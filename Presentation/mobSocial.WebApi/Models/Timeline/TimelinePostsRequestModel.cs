using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Timeline
{
    public class TimelinePostsRequestModel : RootModel
    {
        public int CustomerId { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }
    }
}
