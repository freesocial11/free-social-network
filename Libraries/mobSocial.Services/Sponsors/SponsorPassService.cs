using System;
using System.Collections.Generic;
using mobSocial.Core.Data;
using mobSocial.Core.Services;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Entity.Sponsors;
using mobSocial.Data.Enum;
using mobSocial.Services.Sponsors;

namespace mobSocial.Services.Battles
{
    public class SponsorPassService : BaseEntityService<SponsorPass>, ISponsorPassService
    {

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IWebHelper _webHelper;
        private readonly mobSocialSettings _mobSocialSettings;

        public SponsorPassService(IDataRepository<SponsorPass> repository,
             IDataRepository<Sponsor> sponsorRepository,
             IWorkContext workContext,
             IStoreContext storeContext,
             IOrderService orderService,
             IProductService productService,
             IWebHelper webHelper,
             mobSocialSettings mobSocialSettings)
            : base(repository)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _orderService = orderService;
            _productService = productService;
            _webHelper = webHelper;
            _mobSocialSettings = mobSocialSettings;
        }


        public override List<SponsorPass> GetAll(string term, int count = 15, int page = 1)
        {
            throw new System.NotImplementedException();
        }

        public Nop.Core.Domain.Orders.Order GetSponsorPassOrder(int sponsorPassOrderId)
        {
            var order = _orderService.GetOrderById(sponsorPassOrderId);
            return order;
        }

        public IList<Order> GetSponsorPassOrders(int sponsorCustomerId, int battleId, BattleType battleType)
        {
            
            var orderIds = Repository.Table.Where(x => x.CustomerId == sponsorCustomerId && x.Status == PassStatus.Used && x.BattleType == battleType && x.BattleId == battleId)
                .Select(x => x.SponsorPassOrderId);

            var orders = _orderService.GetOrdersByIds(orderIds.ToArray());
            return orders;


        }
        
        public IList<SponsorPass> GetPurchasedSponsorPasses(int customerId, Enums.PassStatus? sponsorPassStatus)
        {
            var passes = Repository.Table.Where(x => x.CustomerId == customerId);
            if (sponsorPassStatus.HasValue)
                passes = passes.Where(x => x.Status == sponsorPassStatus);
            return passes.ToList();
        }

        public int CreateSponsorPass(BattleType battleType, int battleId, MobSocialProcessPaymentResultModel paymentResponse, UserPaymentMethod paymentMethod, decimal amount)
        {
            //first we'll create an order, let's first get the associated product for voter pass
            var voterPassProduct = GetSponsorPassProduct();

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
                PaymentMethodSystemName = paymentResponse.PaymentMethodSystemName,
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
            var sponsorPass = new SponsorPass() {
                CustomerId = _workContext.CurrentCustomer.Id,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Status = PassStatus.NotUsed,
                SponsorPassOrderId = order.Id,
                BattleType = battleType,
                BattleId = battleId
            };
            //save this pass details
            this.Insert(sponsorPass);

            return order.Id;
        }

        private Product GetSponsorPassProduct()
        {
            var sponsorPass = _productService.GetProductBySku(MobSocialConstant.SponsorPassSKU);
            if (sponsorPass != null) return sponsorPass;
            //the product doesn't exist...so let's create the product first
            sponsorPass = new Product() {
                Name = "Sponsor Pass",
                ProductType = ProductType.SimpleProduct,
                ShowOnHomePage = false,
                Sku = MobSocialConstant.SponsorPassSKU,
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
            _productService.InsertProduct(sponsorPass);

            return sponsorPass;
        }

        public void MarkSponsorPassUsed(int sponsorPassOrderId, int battleId, BattleType battleType)
        {
            var sponsorPass = GetSponsorPassByOrderId(sponsorPassOrderId);
            sponsorPass.Status = PassStatus.Used;
            //make the passes usable interchangeably for picture and video battles
            sponsorPass.BattleType = battleType;
            sponsorPass.BattleId = battleId;
            this.Update(sponsorPass);
        }

        public SponsorPass GetSponsorPassByOrderId(int orderId)
        {
            return Repository.Table.FirstOrDefault(x => x.SponsorPassOrderId == orderId);
        }
    }
}