using mobSocial.Core.Config;

namespace mobSocial.Plugins.PaypalDirect
{
    public class PayPalDirectSettings : ISettingGroup
    {
        /// <summary>
        /// Is sendbox mode enabled
        /// </summary>
        public bool SandboxMode { get; set; }

        /// <summary>
        /// Account name of the paypal api 
        /// </summary>
        public string AccountName { get; set; }

        public string AccountPassword { get; set; }

        public string AccountSignature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
    }
}