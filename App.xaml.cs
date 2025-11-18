
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.Subscription;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Notifications;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Subscription;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Payments.Paystack;

namespace OvulaeApp
{
    public partial class App : Application
    {
        private readonly IOneSignalNotificationService _fcmService;
        private readonly IUsersApi _usersApi;
        private readonly IUserLocalService _usersServ;
        private readonly ISubscriptionService _subscriptionServ;
        private readonly IPaystackApi _paystackApi;

        public App(IUsersApi usersApi, IUserLocalService usersServ, ISubscriptionService subscriptionServ, IPaystackApi paystackApi, IOneSignalNotificationService fcmService)
        {
            InitializeComponent();

            _usersApi = usersApi;
            _usersServ = usersServ;
            _subscriptionServ = subscriptionServ;
            _paystackApi = paystackApi;
            _fcmService = fcmService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnAppLinkRequestReceived(Uri uri)
        {
            try
            {
                base.OnAppLinkRequestReceived(uri);
                if (uri.Host == "payment-success")
                {
                    var primaryGoal = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;
                    var moduleType = primaryGoal != null ? EnumHelper.GetEnumValueFromName<ModuleType>(primaryGoal) : ModuleType.PeriodTracker;
                    LocalStorageService.AppPrimaryGoal = moduleType;

                    var dashboardPage = NavigationsHelper.GetDashboardPageName(primaryGoal);
                    await Shell.Current.GoToAsync(dashboardPage);
                }
            }
            catch
            {
                
            }
        }

        //protected override void OnResume()
        //{
        //    try
        //    {
        //        base.OnResume();

        //        if (Preferences.Get("IsProcessingPayment", false))
        //        {
        //            MainThread.BeginInvokeOnMainThread(async () =>
        //            {
        //                await VerifyPaymentAfterResume();
        //            });
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        protected override void OnStart()
        {
            _ = VerifyPaymentAfterResumeSafe();
        }

        protected override void OnResume()
        {
            _ = VerifyPaymentAfterResumeSafe();
        }

        private async Task VerifyPaymentAfterResume()
        {
            try
            {
                if (LocalStorageService.UserSubscription.LastPaymentReference != null)
                {
                    var payStatus = await _paystackApi.CheckPaymentStatusAsync(LocalStorageService.UserSubscription.LastPaymentReference);

                    if (payStatus.Success)
                    {
                        await _usersServ.ReloadUserData();
                        var dashboardPage = NavigationsHelper.GetDashboardPageName(LocalStorageService.UserCycleProfile.OvulaePrimaryGoal);

                        //manual fallback to update subscription if api webhook failed
                        await _subscriptionServ.RunSubscriptionCheck(true);

                        await Shell.Current.GoToAsync(dashboardPage);
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(nameof(PaymentWallPage));
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Info", "Payment not confirmed yet. Please try again later.", "OK"));
                    }
                }
            }
            catch (Exception ex)
            {
                //await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"Could not verify payment: {ex.Message}", "OK"));
            }
            finally
            {
                Preferences.Remove("IsProcessingPayment");
            }
        }

        private async Task<bool> VerifyOnceAsync(string reference)
        {
            var payStatus = await _paystackApi.CheckPaymentStatusAsync(reference);
            if (payStatus.Success)
            {
                await _usersServ.ReloadUserData();
                await _subscriptionServ.RunSubscriptionCheck(true);
                return true;
            }
            return false;
        }

        private async Task VerifyPaymentAfterResumeSafe()
        {
            try
            {
                if (!Preferences.Get("IsProcessingPayment", false)) return;

                var reference = await SecureStorage.GetAsync("pending_paystack_ref")
                                ?? LocalStorageService.UserSubscription.LastPaymentReference;
                if (string.IsNullOrEmpty(reference)) return;

                // 3 quick retries (1.5s apart)
                for (int i = 0; i < 3; i++)
                {
                    var success = await VerifyOnceAsync(reference);
                    if (success)
                    {
                        var dash = NavigationsHelper.GetDashboardPageName(LocalStorageService.UserCycleProfile.OvulaePrimaryGoal);
                        await Shell.Current.GoToAsync(dash);
                        Preferences.Remove("IsProcessingPayment");
                        SecureStorage.Remove("pending_paystack_ref");
                        return;
                    }
                    await Task.Delay(1500);
                }

                // Not confirmed yet
                await Shell.Current.GoToAsync(nameof(PaymentWallPage));
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Info", "Payment not confirmed yet. Please try again in a moment.", "OK"));
                // Keep flags so next launch/resume retries, or set a TTL you check against
            }
            catch { /* log */ }
        }
    }
}