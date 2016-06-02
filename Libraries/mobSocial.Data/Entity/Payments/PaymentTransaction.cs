using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using mobSocial.Core.Data;
using mobSocial.Data.Enum;
using mobSocial.Data.Interfaces;
using Newtonsoft.Json;

namespace mobSocial.Data.Entity.Payments
{
    public class PaymentTransaction : BaseEntity, ISoftDeletable
    {
        public Guid TransactionGuid { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime CreatedOn { get; set; }

        public int UserId { get; set; }

        public int BillingAddressId { get; set; }

        public decimal TransactionAmount { get; set; }

        public string UserIpAddress { get; set; }

        public int UserPaymentMethodId { get; set; }

        public string PaymentProcessorSystemName { get; set; }
        
        private Dictionary<string, object> _transactionCodes;

        [NotMapped]
        public Dictionary<string, object> TransactionCodes
        {
            get
            {
                _transactionCodes = _transactionCodes ?? new Dictionary<string, object>();
                return _transactionCodes;
            }
            set { _transactionCodes = value; }
        }

        //use serialized string to store data
        public string TransactionCodesSerialized => JsonConvert.SerializeObject(TransactionCodes);

        /// <summary>
        /// Specifies if the transaction is a local transaction or a transaction for third party website
        /// </summary>
        public bool IsLocalTransaction { get; set; }

        public int ThirdPartyAppId { get; set; }

        public bool Deleted { get; set; }
    }

    public class PaymentTransactionMap : BaseEntityConfiguration<PaymentTransaction>
    {
    }
}