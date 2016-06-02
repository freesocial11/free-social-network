using System;
using mobSocial.Core.Data;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Enum;

namespace mobSocial.Data.Entity.Credits
{
    public class Credit : BaseEntity
    {
        public int UserId { get; set; }

        public CreditType CreditType { get; set; }

        public CreditTransactionType CreditTransactionType { get; set; }

        /// <summary>
        /// Specifies a context key to identify the context of this credit entity
        /// </summary>
        public string CreditContextKey { get; set; }

        public decimal CreditCount { get; set; }

        public decimal CreditExchangeRate { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime ExpiresOnUtc { get; set; }

        public bool IsExpired { get; set; }

        public string Remarks { get; set; }

        public int? PaymentTransactionId { get; set; }

        public virtual PaymentTransaction PaymentTransaction { get; set; }

    }

    public class CreditMap : BaseEntityConfiguration<Credit>
    {
        public CreditMap()
        {
            HasOptional(x => x.PaymentTransaction);
        }
    }
}