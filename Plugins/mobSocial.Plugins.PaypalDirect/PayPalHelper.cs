using System;
using System.Linq;
using System.Text;
using PayPal.PayPalAPIInterfaceService.Model;

namespace mobSocial.Plugins.PaypalDirect
{
    public class PayPalHelper
    {
        /// <summary>
        /// Gets the Paypal currency code
        /// </summary>
        public static CurrencyCodeType GetPaypalCurrency(string currencyIsoCode)
        {
            var currencyCodeType = CurrencyCodeType.USD;
            try
            {
                currencyCodeType = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), currencyIsoCode, true);
            }
            catch
            {
                // ignored
            }
            return currencyCodeType;
        }

        /// <summary>
        /// Gets appropriate paypal direct credit card type for given string type
        /// </summary>
        public static CreditCardTypeType GeCreditCardTypeType(string creditCardType)
        {
            switch (creditCardType.ToLowerInvariant())
            {
                case "amex":
                    return CreditCardTypeType.AMEX;
                case "discover":
                    return CreditCardTypeType.DISCOVER;
                case "visa":
                    return CreditCardTypeType.VISA;
                case "mastercard":
                    return CreditCardTypeType.MASTERCARD;
                case "maestro":
                    return CreditCardTypeType.MAESTRO;
                case "switch":
                    return CreditCardTypeType.SWITCH;
                case "solo":
                    return CreditCardTypeType.SOLO;
                default:
                    throw new ArgumentOutOfRangeException(nameof(creditCardType), creditCardType, null);
            }
        }
        /// <summary>
        /// Parses paypal response and returns true if operation succeeds. False otherwise.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ParseResponseSuccess(AbstractResponseType response, out string error)
        {
            var success = false;
            if (response.Ack.HasValue)
            {
                switch (response.Ack)
                {
                    case AckCodeType.SUCCESS:
                    case AckCodeType.SUCCESSWITHWARNING:
                        success = true;
                        break;
                }
            }

            var builder = new StringBuilder();
            var errorFormat = "LongMessage: {0}" + Environment.NewLine + "ShortMessage: {1}" + Environment.NewLine + "ErrorCode: {2}" + Environment.NewLine;
            if (response.Errors != null)
            {
                foreach (var errorType in response.Errors)
                {
                    if (builder.Length <= 0)
                    {
                        builder.Append(Environment.NewLine);
                    }

                    builder.Append(string.Format(errorFormat, errorType.LongMessage, errorType.ShortMessage,
                        errorType.ErrorCode));
                }
            }
            error = builder.ToString();

            return success;
        }
    }
}