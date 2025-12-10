using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using OvulaeApp.Models.IOS;
using OvulaeShared.Enums.Status;
using OvulaeShared.Helpers.CommonFunctions;
using System.Globalization;
using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.Services.Payments
{
    public class SubscriptionPaymentService : ISubscriptionPaymentService
    {
#if IOS
        private IInAppPurchaseService _iapService => ServiceHelper.GetService<IInAppPurchaseService>();
#endif
        public SubscriptionPaymentService()
        {

        }

        public async Task<(string Price, string Currency)> GetSubscriptionPrice()
        {
            try
            {
                var productId = "com.ovulae.org.ovulaeapp.monthlypremium";
                string price = "";
                string currency = "";
#if IOS
                var product = await _iapService.GetProductInfo(productId);
            
                if (product == null)
                {
                    var amount = "$5.99";
                    currency = CountryCodeFunctions.SubscriptionPricing.GetCurrencySymbol(amount);
                    price = amount;
                    return (price, currency);
                }

                // Extract numeric price and currency symbol
                price = product.LocalizedPrice;
                currency = CountryCodeFunctions.SubscriptionPricing.GetCurrencySymbol(product.LocalizedPrice);
#elif ANDROID
                var amount = CountryCodeFunctions.SubscriptionPricing.GetSubscriptionPrice(LocalStorageService.UserDetails.CountryCode);
                currency = CountryCodeFunctions.SubscriptionPricing.GetCurrencySymbol(amount);
                price = amount;
#endif
                return (price, currency);
            }
            catch(Exception ex)
            {
                return ("$4.99", "$");
            }
        }

        public async Task<StatusType> PurchaseSubscription()
        {
            try
            {
                var productId = "com.ovulae.org.ovulaeapp.monthlypremium";
                bool success = false;
#if IOS
                success = await _iapService.PurchaseSubscription(productId);
#endif
                return success ? StatusType.Success : StatusType.Failed;
            }
            catch (Exception ex)
            {
                return StatusType.Error;
            }
        }

        public async Task<StatusType> RestorePurchases()
        {
            try
            {
                bool success = false;
#if IOS
                success = await _iapService.RestorePurchases();
#endif
                return success ? StatusType.Success : StatusType.Failed;
            }
            catch
            {
                return StatusType.Error;
            }
        }

        public async Task<double> GetConvertedPriceForOvulaeZarOrUsd(string localizedPrice = null)
        {
            if (string.IsNullOrEmpty(localizedPrice))
            {
                (localizedPrice, string Currency) = await GetSubscriptionPrice();
            }
            return await CountryCodeFunctions.SubscriptionPricing.GetConvertedPriceForOvulaeZarOrUsd(localizedPrice);
        }

        public class ReceiptInfo
        {
            [JsonPropertyName("in_app")]
            public List<InAppPurchase> InAppPurchases { get; set; }
        }
    }
}
