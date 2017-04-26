using System;
using System.Collections.Generic;
using mobSocial.WebApi.Models.Abstract;
using mobSocial.WebApi.Models.Pictures;

namespace mobSocial.WebApi.Models.ArtistPages
{
    public class ArtistPageModel : AbstractPageModel
    {
        public ArtistPageModel()
        {

        }

        public string RemoteSourceName { get; set; }

        public string RemoteEntityId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string HomeTown { get; set; }

        public string ShortDescription { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public IList<PictureResponseModel> Pictures { get; set; }
    }
}
