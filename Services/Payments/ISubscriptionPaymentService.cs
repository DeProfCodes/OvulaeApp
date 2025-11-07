using OvulaeShared.Enums.Status;

namespace OvulaeApp.Services.Payments
{
    public interface ISubscriptionPaymentService
    {
        public Task<StatusType> PurchaseSubscription();

        public Task<StatusType> RestorePurchases();

        public Task<(string Price, string Currency)> GetSubscriptionPrice();

        public Task<double> GetConvertedPriceForOvulaeZarOrUsd(string localizedPrice = null);
    }
}