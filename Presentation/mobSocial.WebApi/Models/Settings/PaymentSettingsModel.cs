using mobSocial.Data.Enum;

namespace mobSocial.WebApi.Models.Settings
{
    public class PaymentSettingsModel
    {
        public decimal CreditExchangeRate { get; set; }
     
        public int PromotionalCreditUsageLimitPerTransaction { get; set; }
    
        public bool IsPromotionalCreditUsageLimitPercentage { get; set; }
      
        public PaymentMethodSelectionType PaymentMethodSelectionType { get; set; }

        public decimal MicroPaymentsFixedPaymentProcessingFee { get; set; }

        public decimal MacroPaymentsFixedPaymentProcessingFee { get; set; }

        public decimal MicroPaymentsPaymentProcessingPercentage { get; set; }

        public decimal MacroPaymentsPaymentProcessingPercentage { get; set; }

    }
}