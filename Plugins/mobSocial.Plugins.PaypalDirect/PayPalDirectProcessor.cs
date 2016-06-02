using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using mobSocial.Core;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.Mvc;
using mobSocial.Core.Plugins;
using mobSocial.Core.Plugins.Extensibles.Payments;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Payments;
using mobSocial.Services.Extensions;
using mobSocial.Services.Users;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace mobSocial.Plugins.PaypalDirect
{
    public class PayPalDirectProcessor : BasePlugin, IPaymentProcessorPlugin
    {
        private const string ApiVersion = "";

        private readonly PayPalDirectSettings _payPalDirectSettings;
        private readonly IUserService _userService;

        public PayPalDirectProcessor(PayPalDirectSettings payPalDirectSettings, IUserService userService)
        {
            _payPalDirectSettings = payPalDirectSettings;
            _userService = userService;
        }

        public override RouteData GetConfigurationPageRouteData()
        {
            throw new System.NotImplementedException();
        }

        public override RouteData GetDisplayPageRouteData()
        {
            throw new System.NotImplementedException();
        }

        public ITransactionProcessResult Process(ITransactionProcessRequest request, bool authorizeOnly = false)
        {
            var user = _userService.Get(request.UserId);
            if (user == null)
            {
                throw new mobSocialException($"Can't find the user with Id {request.UserId}");
            }
            var creditCard = new CreditCardDetailsType() {
                CreditCardNumber = request.GetParameterAs<string>(PaymentParameterNames.CardNumber),
                CVV2 = request.GetParameterAs<string>(PaymentParameterNames.SecurityCode),
                ExpMonth = request.GetParameterAs<int>(PaymentParameterNames.ExpireMonth),
                ExpYear = request.GetParameterAs<int>(PaymentParameterNames.ExpireYear),
                CreditCardType = PayPalHelper.GeCreditCardTypeType(request.GetParameterAs<string>(PaymentParameterNames.CardIssuerType)),
                CardOwner = new PayerInfoType()
            };


            var paypalCurrency = PayPalHelper.GetPaypalCurrency(request.CurrencyIsoCode);
            var doDirectPaymentRequestDetails = new DoDirectPaymentRequestDetailsType() {
                IPAddress = WebHelper.GetClientIpAddress(),
                PaymentAction = authorizeOnly ? PaymentActionCodeType.AUTHORIZATION : PaymentActionCodeType.SALE,
                CreditCard = creditCard,
                PaymentDetails = new PaymentDetailsType() {
                    OrderTotal = new BasicAmountType() {
                        value = Math.Round(request.Amount, 2).ToString("N", new CultureInfo("en-US")),
                        currencyID = paypalCurrency
                    },
                    Custom = request.TransactionUniqueId,
                    ButtonSource = "mobSocial"
                }
            };

            var doDirectPaymentRequest = new DoDirectPaymentRequestType {
                Version = ApiVersion,
                DoDirectPaymentRequestDetails = doDirectPaymentRequestDetails
            };

            var paymentRequest = new DoDirectPaymentReq {
                DoDirectPaymentRequest = doDirectPaymentRequest
            };

            var service = GetPayPalApiInterfaceServiceService();
            var paypalResponse = service.DoDirectPayment(paymentRequest);

            var result = new TransactionResult();

            string error;
            var success = PayPalHelper.ParseResponseSuccess(paypalResponse, out error);
            if (success)
            {
                result.Success = true;
                result.SetParameter(PaymentParameterNames.AvsCode, paypalResponse.AVSCode);
                result.SetParameter(PaymentParameterNames.Cvv2Code, paypalResponse.CVV2Code);

                if (authorizeOnly)
                {
                    result.SetParameter(PaymentParameterNames.AuthorizationId, paypalResponse.TransactionID);
                    result.SetParameter(PaymentParameterNames.AuthorizationResult, paypalResponse.Ack);
                }
                else
                {
                    result.SetParameter(PaymentParameterNames.CaptureId, paypalResponse.TransactionID);
                    result.SetParameter(PaymentParameterNames.CaptureResult, paypalResponse.Ack);
                }
            }
            else
            {
                result.SetParameter(PaymentParameterNames.ErrorMessage, error);
            }

            return result;
        }

        /// <summary>
        /// Not implemented. Throws exception
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ITransactionPostProcessResult PostProcess(ITransactionPostProcessRequest request)
        {
            throw new System.NotImplementedException();
        }

        public ITransactionCaptureResult Capture(ITransactionCaptureRequest request)
        {
            //to capture a transaction, we'll need authorization id previously obtained from paypal
            var authorizationId = request.GetParameterAs<string>(PaymentParameterNames.AuthorizationId);

            var paypalCurrency = PayPalHelper.GetPaypalCurrency(request.CurrencyIsoCode);

            //create a capture request for paypal
            var doCaptureRequest = new DoCaptureReq() {
                DoCaptureRequest = new DoCaptureRequestType() {
                    Version = ApiVersion,
                    AuthorizationID = authorizationId,
                    Amount = new BasicAmountType() {
                        value = Math.Round(request.Amount, 2).ToString("N", new CultureInfo("en-us")),
                        currencyID = paypalCurrency
                    },
                    CompleteType = CompleteCodeType.COMPLETE
                }
            };

            //get the service for paypal api
            var service = GetPayPalApiInterfaceServiceService();
            var paypalResponse = service.DoCapture(doCaptureRequest);


            var result = new TransactionResult();

            string error;
            var success = PayPalHelper.ParseResponseSuccess(paypalResponse, out error);
            if (success)
            {
                result.Success = true;
                result.SetParameter(PaymentParameterNames.CaptureId, paypalResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID);
                result.SetParameter(PaymentParameterNames.CaptureResult, paypalResponse.Ack);
            }
            else
            {
                result.SetParameter(PaymentParameterNames.ErrorMessage, error);
            }

            return result;
        }

        public ITransactionRefundResult Refund(ITransactionRefundRequest request)
        {
            //to refund a transaction, we'll need capture id previously obtained from paypal
            var captureId = request.GetParameterAs<string>(PaymentParameterNames.CaptureId);

            var paypalCurrency = PayPalHelper.GetPaypalCurrency(request.CurrencyIsoCode);

            //create a capture request for paypal
            var doRefundRequest = new RefundTransactionReq() {
                RefundTransactionRequest = new RefundTransactionRequestType() {
                    Version = ApiVersion,
                    TransactionID = captureId,
                    Amount = new BasicAmountType() {
                        value = Math.Round(request.Amount, 2).ToString("N", new CultureInfo("en-us")),
                        currencyID = paypalCurrency
                    },
                    RefundType = request.IsPartialRefund ? RefundType.PARTIAL : RefundType.FULL
                }
            };

            //get the service for paypal api
            var service = GetPayPalApiInterfaceServiceService();
            var paypalResponse = service.RefundTransaction(doRefundRequest);


            var result = new TransactionResult();

            string error;
            var success = PayPalHelper.ParseResponseSuccess(paypalResponse, out error);
            if (success)
            {
                result.Success = true;
                result.SetParameter(PaymentParameterNames.RefundId, paypalResponse.RefundTransactionID);
                result.SetParameter(PaymentParameterNames.RefundResult, paypalResponse.Ack);
            }
            else
            {
                result.SetParameter(PaymentParameterNames.ErrorMessage, error);
            }

            return result;
        }

        public ITransactionVoidResult Void(ITransactionVoidRequest request)
        {

            //to void a transaction, we'll need capture id or authorization id previously obtained from paypal
            var transactionId = request.GetParameterAs<string>(PaymentParameterNames.AuthorizationId);
            if(string.IsNullOrEmpty(transactionId))
                transactionId = request.GetParameterAs<string>(PaymentParameterNames.CaptureId);

            //create a capture request for paypal
            var doVoidRequest = new DoVoidReq() {
                DoVoidRequest = new DoVoidRequestType() {
                    Version = ApiVersion,
                    AuthorizationID = transactionId
                }
            };

            //get the service for paypal api
            var service = GetPayPalApiInterfaceServiceService();
            var paypalResponse = service.DoVoid(doVoidRequest);


            var result = new TransactionResult();

            string error;
            var success = PayPalHelper.ParseResponseSuccess(paypalResponse, out error);
            if (success)
            {
                result.Success = true;
                result.SetParameter(PaymentParameterNames.RefundId, paypalResponse.AuthorizationID);
                result.SetParameter(PaymentParameterNames.RefundResult, paypalResponse.Ack);
            }
            else
            {
                result.SetParameter(PaymentParameterNames.ErrorMessage, error);
            }

            return result;
        }

        public bool AreParametersValid(Dictionary<string, object> parameters)
        {
            var keysRequired = new[]
            {
                PaymentParameterNames.CardNumber,
                PaymentParameterNames.CardIssuerType,
                PaymentParameterNames.SecurityCode,
                PaymentParameterNames.ExpireMonth,
                PaymentParameterNames.ExpireYear,
                PaymentParameterNames.NameOnCard,
                PaymentParameterNames.Amount,
                PaymentParameterNames.CurrencyIsoCode
            };

            //check if all required keys are present
            var intersection = parameters.Keys.Intersect(keysRequired);
            var valid = intersection.Count() == keysRequired.Length;

            return valid;

        }

        public bool VoidSupported => true;
        public bool RefundSupported => true;
        public bool CaptureSupported => true;

        public PaymentMethodType[] SupportedMethodTypes => new[]
        {
            PaymentMethodType.CreditCard, PaymentMethodType.DebitCard
        };

        protected PayPalAPIInterfaceServiceService GetPayPalApiInterfaceServiceService()
        {
            var config = new Dictionary<string, string>();
            var url = _payPalDirectSettings.SandboxMode ? "https://api-3t.sandbox.paypal.com/2.0" : "https://api-3t.paypal.com/2.0";
            var mode = _payPalDirectSettings.SandboxMode ? "sandbox" : "live";

            config.Add("PayPalAPI", url);
            config.Add("mode", mode);
            config.Add("account0.apiUsername", _payPalDirectSettings.AccountName);
            config.Add("account0.apiPassword", _payPalDirectSettings.AccountPassword);
            config.Add("account0.apiSignature", _payPalDirectSettings.AccountSignature);

            var service = new PayPalAPIInterfaceServiceService(config);
            return service;
        }
    }
}
