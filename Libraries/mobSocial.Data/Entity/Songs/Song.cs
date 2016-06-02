using mobSocial.Core.Data;
using mobSocial.Data.Entity.ArtistPages;
using mobSocial.Data.Interfaces;

namespace mobSocial.Data.Entity.Songs
{
    public class Song : BaseEntity, IPermalinkSupported
    {
        public int PageOwnerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string RemoteEntityId { get; set; }

        public string RemoteSourceName { get; set; }

        public string PreviewUrl { get; set; }

        public string TrackId { get; set; }

        public string RemoteArtistId { get; set; }

        public int? ArtistPageId { get; set; }

        public int AssociatedProductId { get; set; }

        public bool Published { get; set; }

        public virtual ArtistPage ArtistPage { get; set; }
    }
}
