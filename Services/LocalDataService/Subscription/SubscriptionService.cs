using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.User;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Payments;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Payments.Paystack;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OvulaeApp.Services.LocalDataService.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserLocalService _usersServ;
        private readonly IUsersApi _usersApi;
        private readonly IPaystackApi _paystackApi;
        private readonly IPaymentsApi _paymentsApi;

        public SubscriptionService(IUserLocalService usersServ, IUsersApi usersApi, IPaystackApi paystackApi, IPaymentsApi paymentsApi)
        {
            _usersServ = usersServ;
            _usersApi = usersApi;
            _paystackApi = paystackApi;
            _paymentsApi = paymentsApi;
        }

        private async Task<UserSubscription> GetUserSubscription()
        {
            try
            {
                var subscription = LocalStorageService.UserSubscription;
                if (subscription == null || subscription.Id == 0)
                {
                    await _usersServ.ReloadUserData();
                    subscription = LocalStorageService.UserSubscription;
                }
                return subscription;
            }
            catch
            {
                
            }
            return null;
        }

        public async Task RunSubscriptionCheck(bool forceActivate = false)
        {
            try
            {
                var subscription = LocalStorageService.UserSubscription;
                if (subscription != null && subscription.Status != StatusType.Active)
                {
                    var recentUpdate = subscription.LastUpdateDate > DateTime.UtcNow.AddDays(-29);

                    if (recentUpdate && subscription.Status != StatusType.Active && !string.IsNullOrEmpty(subscription.LastPaymentReference))
                    {
                        var checkPaystatus = forceActivate;
                        if (!checkPaystatus)
                        {
                            checkPaystatus = (await _paystackApi.CheckPaymentStatusAsync(subscription.LastPaymentReference)).Success;
                        }

                        if (checkPaystatus)
                        {
                            var forceSubscriptionActivation = await _paymentsApi.RetrySubscriptionPaySuccess(subscription.LastPaymentReference);

                            if (forceSubscriptionActivation.Success)
                            {
                                await _usersServ.ReloadUserData();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
