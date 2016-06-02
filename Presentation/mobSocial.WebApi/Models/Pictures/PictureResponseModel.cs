using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Pictures
{
    public class PictureResponseModel : RootModel
    {
        public string PictureUrl { get; set; }

        public int Id { get; set; }
    }
}