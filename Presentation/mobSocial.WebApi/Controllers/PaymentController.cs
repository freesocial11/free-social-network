using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using mobSocial.Core;
using mobSocial.Core.Plugins.Extensibles.Payments;
using mobSocial.Data.Constants;
using mobSocial.Data.Entity.Credits;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Entity.Settings;
using mobSocial.Services.Battles;
using mobSocial.Services.Payments;
using mobSocial.Services.Sponsors;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.Payment;
using mobSocial.Data.Enum;
using mobSocial.Services.Credits;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.Plugins;
using mobSocial.Services.Security;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Extensions;
using Microsoft.Owin;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("payment")]
    public class PaymentController : RootApiController
    {
        private readonly IUserPaymentMethodService _paymentMethodService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly ICryptographyService _cryptographyService;
        private readonly IVideoBattleService _videoBattleService;
        private readonly ISponsorService _sponsorService;
        private readonly ICreditService _creditService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentController(IUserPaymentMethodService paymentMethodService, 
            IPaymentProcessingService paymentProcessingService, 
            ICryptographyService cryptographyService, 
            IVideoBattleService videoBattleService, 
            ISponsorService sponsorService, 
            ICreditService creditService, 
            PaymentSettings paymentSettings, 
            IPaymentTransactionService paymentTransactionService)
        {
            _paymentMethodService = paymentMethodService;
            _paymentProcessingService = paymentProcessingService;
            _cryptographyService = cryptographyService;
            _videoBattleService = videoBattleService;
            _sponsorService = sponsorService;
            _creditService = creditService;
            _paymentSettings = paymentSettings;
            _paymentTransactionService = paymentTransactionService;
        }


        [HttpGet]
        [Authorize]
        [Route("getavailablepaymentmethods")]
        public IHttpActionResult GetAvailablePaymentMethods([FromUri] UserPaymentModel requestModel)
        {
            if (requestModel == null || !ModelState.IsValid)
                return Json(new { Success = false, Message = "Invalid data supplied" });
            ;
            //first check if the customer has an address, otherwise first address form will be shown

            var currentUser = ApplicationContext.Current.CurrentUser;

            var battleId = requestModel.BattleId;
            var battleType = requestModel.BattleType;
            var purchaseType = requestModel.PurchaseType;

            //TODO: Remove comment when picture battles are ready
            var battle = _videoBattleService.Get(battleId); // Model.BattleType == BattleType.Video ? _videoBattleService.GetById(Model.BattleId) : null;

            var responseModel = new UserPaymentPublicModel {
                IsAmountVariable = purchaseType == PurchaseType.SponsorPass || battle.CanVoterIncreaseVotingCharge,
                MinimumPaymentAmount = purchaseType == PurchaseType.SponsorPass ? battle.MinimumSponsorshipAmount : battle.MinimumVotingCharge,
                PurchaseType = requestModel.PurchaseType
            };

            //if purchase type is sponsor and customer is already a sponsor, he should not have a minimum amount criteria
            if (purchaseType == PurchaseType.SponsorPass)
            {
                var alreadySponsor = _sponsorService.Get(x => x.BattleId == battleId && x.BattleType == battleType, null).Any();
                if (alreadySponsor)
                    responseModel.MinimumPaymentAmount = 1;
            }

            //get available credits
            responseModel.AvailableCredits = _creditService.GetAvailableCreditsCount(currentUser.Id, null);

            //set the usable credits now
            responseModel.UsableCredits = _creditService.GetUsableCreditsCount(currentUser.Id);

            //let's get the payment methods for the logged in user
            var paymentMethods = _paymentMethodService.GetCustomerPaymentMethods(currentUser.Id);
            foreach (var pm in paymentMethods)
            {
                responseModel.UserPaymentMethods.Add(new System.Web.Mvc.SelectListItem() {
                    Text = pm.NameOnCard + " (" + pm.CardNumberMasked + ")",
                    Value = pm.Id.ToString()
                });
            }
            if (battle.VideoBattleStatus == BattleStatus.Complete)
                return Json(new { Success = false, Message = "Battle completed" });
            //battle should not be complete before payment form can be opened
            return Json(new { Success = true, AvailablePaymentMethods = responseModel });
        }

        [Authorize]
        [HttpPost]
        [Route("processpayment")]
        public IHttpActionResult ProcessPayment(FormCollection parameters)
        {
            //first get the payment processor
            var paymentMethodName = parameters.Get(PaymentParameterNames.PaymentMethodTypeName);
            if (string.IsNullOrEmpty(paymentMethodName))
            {
                VerboseReporter.ReportError("Invalid payment method", "process_payment");
                return RespondFailure();
            }

            //the transaction amount
            decimal amount;
            var amountString = parameters.Get(PaymentParameterNames.Amount) ?? "0";
            decimal.TryParse(amountString, out amount);

            PaymentMethodType methodType;
            if (System.Enum.TryParse(paymentMethodName, out methodType))
            {
                methodType = PaymentMethodType.CreditCard;
            }

            //get the payment processor now
            var paymentProcessor = _paymentProcessingService.GetPaymentProcessorPlugin(amount, methodType);

            if (paymentProcessor == null)
            {
                VerboseReporter.ReportError("Invalid payment method", "process_payment");
                return RespondFailure();
            }

            //convert form collection to dictionary to check if parameters are valid
            var formCollectionDictionary = parameters.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

            var isValid = paymentProcessor.AreParametersValid(formCollectionDictionary);

            UserPaymentMethod paymentMethod = null;
            if (!isValid)
            {
                //the parameters are not valid. but that may also mean that the user is selecting an already saved payment method
                //and so he wouldn't have sent that data again
                var savedPaymentMethodIdString = parameters.Get(PaymentParameterNames.UserSavedPaymentMethodId);
                int savedPaymentMethodId;
                if (int.TryParse(savedPaymentMethodIdString, out savedPaymentMethodId))
                {
                    var userPaymentMethods =
                    _paymentMethodService.Get(x => x.UserId == ApplicationContext.Current.CurrentUser.Id && x.Id == savedPaymentMethodId, null);

                    if (userPaymentMethods.Any())
                    {
                        paymentMethod = userPaymentMethods.First();
                        isValid = true;
                    }
                }
                //still invalid? something is not right then.
                if (!isValid)
                {
                    VerboseReporter.ReportError("Invalid parameters to process payment", "process_payment");
                    return RespondFailure();
                }

            }

            //we save the payment method in our database if it's CreditCard 
            if (paymentProcessor.Supports(PaymentMethodType.CreditCard))
            {

                if (paymentMethod == null)
                {
                    #region saving payment method to database 
                    var creditCardNumber = parameters.Get(PaymentParameterNames.CardNumber);
                    //let's validate the card for level 1 check (luhn's test) first before storing
                    var isCardValid = PaymentCardHelper.IsCardNumberValid(creditCardNumber);
                    //card number
                    if (!isCardValid)
                    {
                        VerboseReporter.ReportError("Invalid card number", "process_payment");
                        return RespondFailure();
                    }
                    //expiration date
                    var expireMonth = parameters.Get(PaymentParameterNames.ExpireMonth);
                    var expireYear = parameters.Get(PaymentParameterNames.ExpireYear);
                    if (!expireYear.IsInteger() || !expireMonth.IsInteger())
                    {
                        VerboseReporter.ReportError("Invalid expiration month or year", "process_payment");
                        return RespondFailure();
                    }
                    //card issuer
                    var cardIssuer = PaymentCardHelper.GetCardTypeFromNumber(creditCardNumber);
                    if (!cardIssuer.HasValue)
                    {
                        VerboseReporter.ReportError("Unsupported card provider", "process_payment");
                        return RespondFailure();
                    }


                    var nameOnCard = parameters.Get(PaymentParameterNames.NameOnCard);
                    //encrypt credit card info to store in db
                    var key = ConfigurationManager.AppSettings.Get("EncryptionKey");
                    var salt = ConfigurationManager.AppSettings.Get("Salt");

                    var cardNumber = _cryptographyService.Encrypt(creditCardNumber, key, salt); //encrypt the card info
                    //fine if the card is valid, but is the card number already in our record, then not possible to save the same again
                    if (_paymentMethodService.DoesCardNumberExist(cardNumber))
                    {
                        VerboseReporter.ReportError("The card number is already saved in records", "process_payment");
                        return RespondFailure();
                    }

                    paymentMethod = new UserPaymentMethod() {
                        UserId = ApplicationContext.Current.CurrentUser.Id,
                        IsVerified = false,
                        PaymentMethodType = PaymentMethodType.CreditCard,
                        CardIssuerType = cardIssuer.ToString().ToLowerInvariant(),
                        CardNumber = creditCardNumber,
                        CardNumberMasked = PaymentCardHelper.MaskCardNumber(creditCardNumber),
                        NameOnCard = nameOnCard,

                    };
                    //save this payment method
                    _paymentMethodService.Insert(paymentMethod);
                    #endregion
                }
            }

            
            //we need to see if we should only authorize or capture as well
            //the idea is if it's a sponsorship context, it's better to authorize the payment transaction and capture later when
            //the sponsorship is accepted //we thought of initially only authorizing sponsorship transactions and capture when it's accepted.
            //but that flow doesn't seem to work quite well, thoughts?
            var authorizeOnly = false; // (parameters.Get(PaymentParameterNames.PaymentContext) ?? string.Empty) == "sponsor";

            //so we are ready for payment processing, let's create a paymenttrasaction for storing in our db
            var paymentTransaction = new PaymentTransaction() {
                IsLocalTransaction = true,
                PaymentStatus = PaymentStatus.Pending,
                TransactionAmount = amount,
                TransactionGuid = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                UserIpAddress = WebHelper.GetClientIpAddress()
            };
            _paymentTransactionService.Insert(paymentTransaction);

            //now proceed further with the payment
            //create the transaction request
            var transactionRequest = new TransactionRequest() {
                Amount = amount,
                CurrencyIsoCode = "USD",//TODO: SET CURRENCY AS SELECTED BY USER
                PaymentProcessorSystemName = paymentProcessor.PluginInfo.SystemName,
                UserId = ApplicationContext.Current.CurrentUser.Id,
                Parameters = formCollectionDictionary,
                TransactionUniqueId = paymentTransaction.TransactionGuid.ToString()
            };


            var response = paymentProcessor.Process(transactionRequest, authorizeOnly);
            //update values of transaction parameters for future reference
            paymentTransaction.TransactionCodes = response.ResponseParameters;
            //update payment transaction
            _paymentTransactionService.Update(paymentTransaction);

            if (response.Success)
            {
                //let's verify the payment method first if it's not
                if (paymentMethod != null && !paymentMethod.IsVerified)
                {
                    paymentMethod.IsVerified = true;
                    _paymentMethodService.Update(paymentMethod);
                }

                //now since the response was success, we can actually assign some credits to the user
                var creditCount = amount * (1 / _paymentSettings.CreditExchangeRate);
                var credit = new Credit()
                {
                    PaymentTransactionId = paymentTransaction.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    CreditCount = creditCount,
                    CreditExchangeRate = _paymentSettings.CreditExchangeRate,
                    //if it's authorize only transaction, we assign the credits, but they won't be usable before they are approved by capture
                    CreditTransactionType = CreditTransactionType.Issued,
                    CreditType = CreditType.Transactional,
                    IsExpired = false
                };

                //save credit
                _creditService.Insert(credit);

                //get total available credits of user
                var usableCreditCount = _creditService.GetUsableCreditsCount(ApplicationContext.Current.CurrentUser.Id);
                return RespondSuccess(new {
                    UsableCreditCount = usableCreditCount
                });
            }
            VerboseReporter.ReportError("An error occured while processing payment", "process_payment");
            return RespondFailure();
        }

      /*  [Authorize]
        [HttpPost]
        [Route("purchasecredits")]
        public IHttpActionResult PurchaseCredits(PurchaseCreditModel requestModel)
        {
            if (requestModel.CustomerPaymentRequest.Amount <= 0)
                return Json(new { Success = false, Message = "Minimum amount to pay should be greater than zero" });

            //check if the payment method provided by customer is new or an existing one
            UserPaymentMethod paymentMethod = null;

            //should we authorize only or should capture as well? for sponsorship pass, it's authorized only & captured later
            var authorizeOnly = requestModel.PurchaseType == PurchaseType.SponsorPass;

            if (requestModel.CustomerPaymentMethodId == 0)
            {
                paymentMethod = new UserPaymentMethod() {
                    UserId = ApplicationContext.Current.CurrentUser.Id,
                    IsVerified = false,
                    PaymentMethodType = requestModel.CustomerPaymentRequest.PaymentMethod
                };

                switch (requestModel.CustomerPaymentRequest.PaymentMethod)
                {
                    case PaymentMethodType.CreditCard:
                    case PaymentMethodType.DebitCard:
                        //if it's a card, it should be valid. why send to payment processor if basic checks fail?

                        var cardNumber = PaymentCardHelper.StripCharacters(requestModel.CustomerPaymentRequest.CardNumber);
                        //let's validate the card for level 1 check (luhn's test) first before storing
                        var isCardValid = PaymentCardHelper.IsCardNumberValid(cardNumber);

                        if (isCardValid)
                        {
                            var cardIssuer = PaymentCardHelper.GetCardTypeFromNumber(cardNumber);
                            var cardNumberMasked = PaymentCardHelper.MaskCardNumber(cardNumber);

                            var key = _cryptographyService.GetSavedEncryptionKey();
                            var salt = _cryptographyService.GetSavedSalt();

                            cardNumber = _cryptographyService.Encrypt(cardNumber, key, salt); //encrypt the card info

                            //fine if the card is valid, but is the card number already in our record, then not possible to save the same again
                            if (_paymentMethodService.DoesCardNumberExist(cardNumber))
                            {
                                return Json(new { Success = false, Message = "The card number is already saved in records" });
                            }
                            //all good so far, but payment method will still be non-verified till first transaction is done.

                            paymentMethod.CardNumber = cardNumber;
                            paymentMethod.CardNumberMasked = cardNumberMasked;
                            paymentMethod.ExpireMonth = requestModel.CustomerPaymentRequest.ExpireMonth;
                            paymentMethod.ExpireYear = requestModel.CustomerPaymentRequest.ExpireYear;
                            paymentMethod.NameOnCard = requestModel.CustomerPaymentRequest.NameOnCard;
                            paymentMethod.CardIssuerType = cardIssuer.ToString().ToLower();
                        }
                        else
                        {
                            return Json(new { Success = false, Message = "Invalid card number" });
                        }
                        break;
                    case PaymentMethodType.BitCoin:
                        //TODO: Bitcoin related data here
                        break;
                    case PaymentMethodType.Paypal:

                        //TODO: Paypal related data here
                        break;
                }

                //save the payment method
                _paymentMethodService.Insert(paymentMethod);
            }
            else
            {
                //so we have a saved method, let's retrieve it
                paymentMethod = _paymentMethodService.Get(requestModel.CustomerPaymentMethodId);

                //okays...but does this payment method actually belong to this customer?
                if (paymentMethod.UserId != ApplicationContext.Current.CurrentUser.Id)
                {
                    return Json(new { Success = false, Message = "Invalid payment method" });
                }

            }
            //so we are good to go with this transaction...let's see how to proceed

            //we need to make sure that purchase amount is at least as minimum as battle
            //TODO: Remove comment when picture battles are ready
            var battle = _videoBattleService.Get(requestModel.BattleId); // Model.BattleType == BattleType.Video ? _videoBattleService.GetById(Model.BattleId) : null;
            if (requestModel.PurchaseType == PurchaseType.VoterPass && requestModel.Amount < battle.MinimumVotingCharge)
            {
                return Json(new { Success = false, Message = "Payment amount is less than minimum voting amount" });
            }

            if (requestModel.PurchaseType == PurchaseType.SponsorPass && requestModel.Amount < battle.MinimumSponsorshipAmount)
            {
                //only if user is not increasing the sponsorship amount should we send error, else accept any amount
                var sponsorshipOrders = _sponsorPassService.GetSponsorPassOrders(ApplicationContext.Current.CurrentUser.Id, requestModel.BattleId, requestModel.BattleType);
                if (!sponsorshipOrders.Any())
                    return Json(new { Success = false, Message = "Payment amount is less than minimum sponsorship amount" });
            }

            //process the payment now, for sponsor pass, we authorize only till sponsorship is approved
            var paymentResponse = _paymentProcessingService.ProcessPayment(ApplicationContext.Current.CurrentUser, paymentMethod, requestModel.Amount, authorizeOnly);
            if (paymentResponse != null && paymentResponse.ProcessPaymentResult.Success)
            {


                switch (requestModel.PurchaseType)
                {
                    case mobSocial.Data.Enum.PurchaseType.VoterPass:
                        //let's create voter pass
                        var voterPassId = _voterPassService.CreateVoterPass(requestModel.BattleType, requestModel.BattleId,
                            paymentResponse, paymentMethod, requestModel.Amount);
                        return Json(new { Success = true, PassId = voterPassId });
                    case mobSocial.Data.Enum.PurchaseType.SponsorPass:
                        //let's create sponsorship pass
                        var sponsorPassId = _sponsorPassService.CreateSponsorPass(requestModel.BattleType, requestModel.BattleId, paymentResponse, paymentMethod, requestModel.Amount);
                        return Json(new { Success = true, PassId = sponsorPassId });
                }
            }
            return Json(new { Success = false, Message = "Payment failed", Errors = paymentResponse != null ? paymentResponse.ProcessPaymentResult.Errors : new List<string>() });

        }*/

    }
}