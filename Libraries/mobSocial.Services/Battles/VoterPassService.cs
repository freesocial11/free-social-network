using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Enum;
using Nop.Plugin.WebApi.MobSocial.Domain;

namespace mobSocial.Services.Battles
{
    public class VoterPassService : BaseEntityService<VoterPass>, IVoterPassService
    {
 
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;
        private readonly mobSocialSettings _mobSocialSettings;

        public VoterPassService(IDataRepository<VoterPass> repository,
            IWorkContext workContext, 
            IStoreContext storeContext, 
            IOrderService orderService, 
            IProductService productService,
            IWebHelper webHelper,
            mobSocialSettings mobSocialSettings) : base(repository)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _orderService = orderService;
            _productService = productService;
            _webHelper = webHelper;
            _mobSocialSettings = mobSocialSettings;
        }

        public Order GetVoterPassOrder(int voterPassOrderId)
        {
            var order = _orderService.GetOrderById(voterPassOrderId);
            return order;
        }

        public IList<VoterPass> GetPurchasedVoterPasses(int customerId, PassStatus? votePassStatus)
        {
            var passes = Repository.Table.Where(x => x.CustomerId == customerId);
            if (votePassStatus.HasValue)
                passes = passes.Where(x => x.Status == votePassStatus);
            return passes.ToList();
        }



        public int CreateVoterPass(BattleType battleType, int battleId, MobSocialProcessPaymentResultModel paymentResponse, UserPaymentMethod paymentMethod, decimal amount)
        {
            //first we'll create an order, let's first get the associated product for voter pass
            var voterPassProduct = GetVoterPassProduct(battleType);

            //place an order on user's behalf
            var order = new Order() {
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                StoreId = _storeContext.CurrentStore.Id,
                BillingAddress = _workContext.CurrentCustomer.Addresses.First(),
                ShippingAddress = _workContext.CurrentCustomer.Addresses.First(),
                AuthorizationTransactionCode = paymentResponse.ProcessPaymentResult.AuthorizationTransactionCode,
                AuthorizationTransactionId = paymentResponse.ProcessPaymentResult.AuthorizationTransactionId,
                AuthorizationTransactionResult = paymentResponse.ProcessPaymentResult.AuthorizationTransactionResult,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                OrderStatus = OrderStatus.Complete,
                PaymentStatus = paymentResponse.ProcessPaymentResult.NewPaymentStatus,
                ShippingStatus = ShippingStatus.ShippingNotRequired,
                PaymentMethodSystemName = "MobSocial.Payments." + paymentMethod.PaymentMethodType.ToString(),
                OrderTotal = amount,
                OrderSubtotalExclTax = amount,
                OrderSubTotalDiscountInclTax = amount,
                OrderGuid = Guid.NewGuid(),
                CustomerCurrencyCode = _workContext.WorkingCurrency.CurrencyCode,
                CurrencyRate = _workContext.WorkingCurrency.Rate
            };
            var orderItem = new OrderItem() {
                OrderItemGuid = Guid.NewGuid(),
                ProductId = voterPassProduct.Id,
                UnitPriceExclTax = amount,
                UnitPriceInclTax = amount,
                PriceInclTax = amount,
                PriceExclTax = amount,
                Quantity = 1
            };
            order.OrderItems.Add(orderItem);
            //save the order now
            _orderService.InsertOrder(order);

            //now add this voter pass for future reference
            var voterPass = new VoterPass()
            {
                BattleId = battleId,
                BattleType = battleType,
                CustomerId = _workContext.CurrentCustomer.Id,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Status = PassStatus.NotUsed,
                VoterPassOrderId = order.Id
            };
            //save this pass details
            this.Insert(voterPass);

            return order.Id;
        }

        public void MarkVoterPassUsed(int voterPassOrderId)
        {
            var voterPass = GetVoterPassByOrderId(voterPassOrderId);
            voterPass.Status = PassStatus.Used;
            this.Update(voterPass);
        }

        public VoterPass GetVoterPassByOrderId(int orderId)
        {
           return Repository.Table.FirstOrDefault(x => x.VoterPassOrderId == orderId);
        }

        public IList<Order> GetAllVoterPassOrders(BattleType battleType, int battleId, PassStatus? voterPassStatus)
        {
            var passes = Repository.Table.Where(x => x.BattleId == battleId && x.BattleType == battleType);
            if (voterPassStatus.HasValue)
                passes = passes.Where(x => x.Status == voterPassStatus.Value);

            var orderIds = passes.Select(x => x.VoterPassOrderId).ToArray();
            return _orderService.GetOrdersByIds(orderIds);
        }

        private Product GetVoterPassProduct(BattleType battleType)
        {
            var voterPass = _productService.GetProductBySku(battleType == Enums.BattleType.Video ? MobSocialConstant.VideoBattleVoterPassSKU : MobSocialConstant.PictureBattleVoterPassSKU);
            if (voterPass == null)
            {
                //the product doesn't exist...so let's create the product first
                voterPass = new Product() {
                    Name = "Voter Pass for " + battleType.ToString(),
                    ProductType = ProductType.SimpleProduct,
                    ShowOnHomePage = false,
                    Sku = battleType == battleType.Video ? MobSocialConstant.VideoBattleVoterPassSKU : MobSocialConstant.PictureBattleVoterPassSKU,
                    VisibleIndividually = false,
                    IsShipEnabled = false,
                    IsDownload = false,
                    IsGiftCard = false,
                    GiftCardType = GiftCardType.Virtual,
                    CustomerEntersPrice = true,
                    MinimumCustomerEnteredPrice = _mobSocialSettings.DefaultVotingChargeForPaidVoting,
                    MaximumCustomerEnteredPrice = 0, //no limit
                    Price = _mobSocialSettings.DefaultVotingChargeForPaidVoting,
                    AllowCustomerReviews = false,
                    SubjectToAcl = false,
                    LimitedToStores = false,
                    HasTierPrices = false,
                    HasDiscountsApplied = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    DisplayOrder = 0,
                    Published = true
                };
                _productService.InsertProduct(voterPass);
            }

            return voterPass;
        }

        public override List<VoterPass> GetAll(string term, int count = 15, int page = 1)
        {
            throw new NotImplementedException();
        }
    }
}