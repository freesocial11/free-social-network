using System.ComponentModel.DataAnnotations.Schema;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.ArtistPages
{
    public class ArtistPageManager : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ArtistPageId { get; set; }

        [ForeignKey("ArtistPageId")]
        public virtual ArtistPage ArtistPage { get; set; }
    }
}
