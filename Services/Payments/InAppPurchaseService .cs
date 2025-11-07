#if IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Foundation;
using StoreKit;
using Newtonsoft.Json;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace OvulaeApp.Services.Payments
{

    internal sealed class ReceiptRefreshRequestDelegate : SKRequestDelegate
    {
        private readonly TaskCompletionSource<bool> _tcs;
        public ReceiptRefreshRequestDelegate(TaskCompletionSource<bool> tcs) => _tcs = tcs;

        public override void RequestFinished(SKRequest request)
            => _tcs.TrySetResult(true);

        public override void RequestFailed(SKRequest request, NSError error)
            => _tcs.TrySetResult(false);
    }

    public interface IInAppPurchaseService
    {
        Task<InAppProduct> GetProductInfo(string productId);
        Task<bool> PurchaseSubscription(string productId);
        Task<bool> RestorePurchases();
        Task<bool> IsSubscribed();
        Task<NSData> GetReceiptDataOrRefreshAsync();
    }

    public class InAppProduct
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string LocalizedPrice { get; set; }
        public string Description { get; set; }
    }

    public class ReceiptVerificationResult
    {
        [JsonProperty("status")]
        public int Status { get; set; }
        
        [JsonProperty("receipt")]
        public ReceiptInfo Receipt { get; set; }
    }

    public class ReceiptInfo
    {
        [JsonProperty("in_app")]
        public List<InAppPurchase> InAppPurchases { get; set; }
    }

    public class InAppPurchase
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
    }

    public class PaymentObserver : SKPaymentTransactionObserver
    {
        public event EventHandler<SKPaymentTransaction[]> TransactionUpdated;

        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
        {
            TransactionUpdated?.Invoke(this, transactions);
        }
    }

    public class RestoreObserver : SKPaymentTransactionObserver
    {
        public event EventHandler RestoreCompleted;
        public event EventHandler<NSError> RestoreFailed;

        public override void RestoreCompletedTransactionsFinished(SKPaymentQueue queue)
        {
            RestoreCompleted?.Invoke(this, EventArgs.Empty);
        }

        public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error)
        {
            RestoreFailed?.Invoke(this, error);
        }
    }

    public partial class InAppPurchaseService : IInAppPurchaseService
    {
        private static PaymentObserver _sharedObserver;     // <— shared for whole app
        private TaskCompletionSource<SKPaymentTransaction> _purchaseTcs;
        private readonly List<string> _productIds = new() { "com.ovulae.org.ovulaeapp.monthlypremium.v3" };

        public InAppPurchaseService()
        {
            EnsureObserver(); // attach once on service creation
        }

        private void EnsureObserver()
        {
            if (_sharedObserver != null) return;
            _sharedObserver = new PaymentObserver();
            _sharedObserver.TransactionUpdated += OnTransactionUpdated;
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(_sharedObserver);
        }

        public async Task<InAppProduct> GetProductInfo(string productId)
        {
            try
            {
                Console.WriteLine($"Fetching product info for: {productId}");
                
                var productIds = new NSSet(_productIds.ToArray());
                var tcs = new TaskCompletionSource<SKProductsResponse>();
                var request = new SKProductsRequest(productIds);
                request.Delegate = new ProductsRequestDelegate(tcs);
                request.Start();
                
                var response = await tcs.Task;
                Console.WriteLine($"Found {response.Products.Length} products");
                
                var product = response.Products.FirstOrDefault(p => p.ProductIdentifier == productId);
                
                if (product == null)
                {
                    Console.WriteLine($"Product {productId} not found in App Store");
                    return null;
                }
        
                Console.WriteLine($"Product found: {product.LocalizedTitle} - {product.LocalizedPrice()}");
                
                return new InAppProduct
                {
                    ProductId = product.ProductIdentifier,
                    Name = product.LocalizedTitle,
                    Description = product.LocalizedDescription,
                    LocalizedPrice = product.LocalizedPrice()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetProductInfo error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> PurchaseSubscription(string productId)
        {
            try
            {
                if (!_productIds.Contains(productId))
                    throw new ArgumentException("Invalid product ID");

                if (!SKPaymentQueue.CanMakePayments)
                    throw new Exception("In-app purchases not enabled");

                // Ensure the shared observer is attached
                EnsureObserver();

                _purchaseTcs = new TaskCompletionSource<SKPaymentTransaction>();
                SKPaymentQueue.DefaultQueue.AddPayment(SKPayment.CreateFrom(productId));

                var transaction = await _purchaseTcs.Task;
                return transaction?.TransactionState == SKPaymentTransactionState.Purchased;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purchase error: {ex.Message}");
                return false;
            }
            // IMPORTANT: do NOT remove the observer here; keep it for the whole session
        }

        private void OnTransactionUpdated(object sender, SKPaymentTransaction[] transactions)
        {
            foreach (var transaction in transactions)
            {
                switch (transaction.TransactionState)
                {
                    case SKPaymentTransactionState.Purchased:
                        _purchaseTcs?.TrySetResult(transaction);
                        CompleteTransaction(transaction);
                        break;
                    case SKPaymentTransactionState.Failed:
                        _purchaseTcs?.TrySetResult(null);
                        SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
                        break;
                    case SKPaymentTransactionState.Restored:
                        CompleteTransaction(transaction);
                        break;
                }
            }
        }

        public async Task<bool> RestorePurchases()
        {
            var tcs = new TaskCompletionSource<bool>();
            var observer = new RestoreObserver();

            observer.RestoreCompleted += (s, e) => tcs.TrySetResult(true);
            observer.RestoreFailed += (s, e) => tcs.TrySetResult(false);

            SKPaymentQueue.DefaultQueue.AddTransactionObserver(observer);
            SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();

            try
            {
                return await tcs.Task;
            }
            finally
            {
                SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(observer);
            }
        }

        public async Task<bool> IsSubscribed()
        {
            var receiptData = await GetReceiptDataOrRefreshAsync();
            return receiptData != null && await VerifyReceiptWithApple(receiptData.ToArray());
        }

        private async void CompleteTransaction(SKPaymentTransaction transaction)
        {
            try
            {
                var receiptData = await GetReceiptDataOrRefreshAsync();
            }
            finally
            {
                SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
            }
        }

        private static Task<bool> RefreshReceiptAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            var req = new SKReceiptRefreshRequest();     // you can pass properties if needed
            req.Delegate = new ReceiptRefreshRequestDelegate(tcs);
            req.Start();
            return tcs.Task;
        }

        public async Task<NSData> GetReceiptDataOrRefreshAsync()
        {
            var url = NSBundle.MainBundle.AppStoreReceiptUrl;
            var data = url != null ? NSData.FromUrl(url) : null;
            if (data == null)
            {
                var ok = await RefreshReceiptAsync();
                if (!ok) return null;
                url = NSBundle.MainBundle.AppStoreReceiptUrl;
                data = url != null ? NSData.FromUrl(url) : null;
            }
            return data;
        }


        private async Task ValidateAndActivatePremium(byte[] receiptData)
        {
            var isValid = await VerifyReceiptWithApple(receiptData);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Preferences.Default.Set("IsPremiumUser", isValid);
                MessagingCenter.Send(this, "PremiumStatusUpdated");
            });
        }

        private async Task<bool> VerifyReceiptWithApple(byte[] receiptData)
        {
            try
            {
                var request = new
                {
                    receipt_data = Convert.ToBase64String(receiptData),
                    password = "652c08f361da4715903c91a861b3fdf5"
                };
        
                using var client = new HttpClient();
                var jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
                var url = "https://buy.itunes.apple.com/verifyReceipt";
                var response = await client.PostAsync(url, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ReceiptVerificationResult>(jsonResponse);
        
                // Retry with sandbox if status is 21007
                if (result?.Status == 21007)
                {
                    url = "https://sandbox.itunes.apple.com/verifyReceipt";
                    response = await client.PostAsync(url, content);
                    jsonResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ReceiptVerificationResult>(jsonResponse);
                }
        
                return result?.Status == 0 &&
                       result.Receipt?.InAppPurchases?.Any(p => _productIds.Contains(p.ProductId)) == true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class SKProductExtensions
    {
        public static string LocalizedPrice(this SKProduct product)
        {
            var formatter = new NSNumberFormatter
            {
                FormatterBehavior = NSNumberFormatterBehavior.Version_10_4,
                NumberStyle = NSNumberFormatterStyle.Currency,
                Locale = product.PriceLocale
            };
            return formatter.StringFromNumber(product.Price);
        }
    }

    public static class SKProductsRequestExtensions
    {
        public static Task<SKProductsResponse> TaskAsync(this SKProductsRequest request)
        {
            var tcs = new TaskCompletionSource<SKProductsResponse>();
            request.ReceivedResponse += (sender, e) => tcs.TrySetResult(e.Response);
            request.RequestFailed += (sender, e) => tcs.TrySetException(new Exception(e.Error.LocalizedDescription));
            request.Start();
            return tcs.Task;
        }
    }

    internal class ProductsRequestDelegate : SKProductsRequestDelegate
    {
        private readonly TaskCompletionSource<SKProductsResponse> _tcs;
    
        public ProductsRequestDelegate(TaskCompletionSource<SKProductsResponse> tcs)
        {
            _tcs = tcs;
        }
    
        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
        {
            Console.WriteLine($"Products received: {response.Products.Length}");
            _tcs.TrySetResult(response);
        }
    
        //public override void RequestFailed(SKProductsRequest request, NSError error)
        //{
        //    Console.WriteLine($"Product request failed: {error.LocalizedDescription}");
        //    _tcs.TrySetException(new Exception(error.LocalizedDescription));
        //}
    }
}
#endif