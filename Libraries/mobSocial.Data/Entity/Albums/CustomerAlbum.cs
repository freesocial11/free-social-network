using System.Collections.Generic;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Albums
{
    public class CustomerAlbum : BaseEntity
    {
        public virtual int CustomerId { get; set; }
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        /// <summary>
        /// The main album photos appear in the Pictures tab
        /// </summary>
        public virtual bool IsMainAlbum { get; set; }



        public virtual List<CustomerAlbumPicture> Pictures { get; set; }
        
    }


}