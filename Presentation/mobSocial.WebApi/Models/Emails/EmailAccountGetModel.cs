
using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Emails
{
    public class EmailAccountGetModel : RootModel
    {
        public int Page { get; set; }

        public int Count { get; set; }
    }
}