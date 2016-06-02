using System;
using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Songs
{
    public class SharedSong : BaseEntity
    {
        public int CustomerId { get; set; }
        public int SenderId { get; set; }
        public int SongId { get; set; }
        public string Message { get; set; }
        public DateTime SharedOn { get; set; }
        public virtual Song Song { get; set; }
       

    }
}
