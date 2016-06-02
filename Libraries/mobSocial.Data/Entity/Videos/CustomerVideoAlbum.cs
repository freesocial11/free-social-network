using System.Collections.Generic;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Videos
{
    public class CustomerVideoAlbum : BaseEntity
    {
        public virtual int CustomerId { get; set; }
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        /// <summary>
        /// The main video album photos appear in the Videos tab
        /// </summary>
        public virtual bool IsMainVideoAlbum { get; set; }



        public virtual List<CustomerVideo> Videos { get; set; }
        
    }


}