using System.Collections.Generic;
using System.Linq;
using mobSocial.Core.Exception;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Plugins.Extensibles.Payments;
using mobSocial.Data.Entity.Payments;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Users;
using mobSocial.Services.Plugins;

namespace mobSocial.Services.Payments
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        const decimal MicroMacroPaymentSwitchingAmount = 11; //< $11.00 micro, >$11.0 macro

        private readonly IPluginFinderService _pluginFinder;
        private readonly PaymentSettings _paymentSettings;

        public PaymentProcessingService(IPluginFinderService pluginFinder, 
            PaymentSettings paymentSettings)
        {
            _pluginFinder = pluginFinder;
            _paymentSettings = paymentSettings;
        }

        private IPaymentProcessorPlugin GetPluginInstance(string pluginSystemName)
        {
            var isInstalled = false;
            var pluginInfo = _pluginFinder.FindPlugin(pluginSystemName);

            if (pluginInfo != null && pluginInfo.Installed)
            {
                isInstalled = true;
            }

            //is installed
            if (!isInstalled)
                return null;

            var plugin = pluginInfo.LoadPluginInstance<IPaymentProcessorPlugin>();
            return plugin;

        }

        public ITransactionProcessResult ProcessPayment(User user, UserPaymentMethod paymentMethod, ITransactionProcessRequest processRequest, bool authorizeOnly = false)
        {
            if(processRequest == null)
                throw new mobSocialException("Can't process null request");

            //check if the plugin is installed and activated
            //depending on the payment amount, we may wish to switch between macro and micro payments
            //check if plugin is installed or not
            //TODO: Change the other payment method to appropriate one for macro payments
            var paymentPluginSystemName = processRequest.Amount < MicroMacroPaymentSwitchingAmount ? "Payments.PayPalDirect" : "Payments.PayPalDirect";

            //if we get an instance, then the plugin is installed
            var plugin = GetPluginInstance(paymentPluginSystemName);

            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture
            if (plugin == null || (!plugin.CaptureSupported && authorizeOnly))
                return null;

           //process the payment now
            var result = plugin.Process(processRequest);
            return result;
        }

        public ITransactionCaptureResult CapturePayment(PaymentTransaction transaction)
        {
            var plugin = GetPluginInstance(transaction.PaymentProcessorSystemName);
            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture/void
            if (plugin == null || !plugin.CaptureSupported)
                return null;

            var captureResult = plugin.Capture(new TransactionRequest() {
                Amount = transaction.TransactionAmount,
                Parameters = transaction.TransactionCodes
            });
            return captureResult;
        }

        public ITransactionVoidResult VoidPayment(PaymentTransaction transaction)
        {
            var plugin = GetPluginInstance(transaction.PaymentProcessorSystemName);
            //plugin should be available and if it's an authorization transaction, the plugin should actually support authorize and capture/void
            if (plugin == null || !plugin.VoidSupported)
                return null;

            var voidResult = plugin.Void(new TransactionRequest() {
                Amount = transaction.TransactionAmount,
                Parameters = transaction.TransactionCodes
            });
            return voidResult;
        }

        public ITransactionRefundResult RefundPayment(PaymentTransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public IPaymentProcessorPlugin GetPaymentProcessorPlugin(decimal amount, PaymentMethodType methodType)
        {
            //TODO: Implement dynamic payment processor options depending on amount
            var paymentProcessorSystemName = "mobSocial.Plugins.PayPalDirect";
            return GetPluginInstance(paymentProcessorSystemName);
        }


        public decimal GetNetAmountAfterPaymentProcessing(decimal amount)
        {
            var fixProcessingFee = amount < MicroMacroPaymentSwitchingAmount
                ? _paymentSettings.MicroPaymentsFixedPaymentProcessingFee
                : _paymentSettings.MacroPaymentsFixedPaymentProcessingFee;

            var processingPercentage = amount < MicroMacroPaymentSwitchingAmount
                ? _paymentSettings.MicroPaymentsPaymentProcessingPercentage
                : _paymentSettings.MacroPaymentsPaymentProcessingPercentage;

            return amount - fixProcessingFee - (amount * processingPercentage / 100);
        }

        public IList<IPaymentProcessorPlugin> GetSupportedPaymentProcessorPlugins(params PaymentMethodType[] methodTypes)
        {
            //load finder service
            var pluginFinderService = mobSocialEngine.ActiveEngine.Resolve<IPluginFinderService>();
            var paymentProcessorPluginInfos = pluginFinderService.FindPlugins<IPaymentProcessorPlugin>();

            var listPlugins = new List<IPaymentProcessorPlugin>();
            foreach (var pluginInfo in paymentProcessorPluginInfos.Where(x => x.Active))
            {
                //instantiate and check if the current plugininfo supports any of the asked method types
                var pluginInstance = pluginInfo.LoadPluginInstance<IPaymentProcessorPlugin>();
                if(pluginInstance.SupportedMethodTypes.Intersect(methodTypes).Any())
                    listPlugins.Add(pluginInstance);
            }
            return listPlugins;
        }
    }
}