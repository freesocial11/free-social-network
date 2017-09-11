using System;
using System.ComponentModel.DataAnnotations;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.OAuth
{
    public class AppToken : BaseEntity
    {
        [Required]
        public string Guid { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string ClientId { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

        public TokenType TokenType { get; set; }
    }

    public class AppTokenMap: BaseEntityConfiguration<AppToken> { }
}