using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Songs;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.ArtistPages
{
    public class ArtistPage : BaseEntity, IPermalinkSupported, IPicturesSupported<ArtistPage>
    {
        public int PageOwnerId { get; set; }

        public string Name { get; set; }

        public string RemoteEntityId { get; set; }

        public string RemoteSourceName { get; set; }

        public string Biography { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string HomeTown { get; set; }

        public string ShortDescription { get; set; }

        public virtual IList<ArtistPageManager> PageManagers { get; set; }

        public virtual IList<Song> Songs { get; set; }

    }
}
