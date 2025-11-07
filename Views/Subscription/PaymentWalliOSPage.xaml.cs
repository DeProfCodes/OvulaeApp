using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Payments;
using OvulaeApp.ViewModels.Subscription;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Services.APIs.Payments;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Payments.Paystack;
using OvulaeShared.ViewModel.IOS;

namespace OvulaeApp.Views.Subscription
{
    public partial class PaymentWalliOSPage : ContentPage
    {
        private readonly IPaymentsApi _paymentsApi;
        private readonly IUserLocalService _usersServ;
        private readonly IUsersApi _userApi;

        private readonly ISubscriptionPaymentService _applePay;

#if IOS
        private IInAppPurchaseService _inAppPurchase = ServiceHelper.GetService<IInAppPurchaseService>();
#endif

        private string price;
        private PaywallViewModel viewModel;

        private bool _isHandlingCheckChange = false;

        bool IsEligibleForFreeTrial = false;

        public PaymentWalliOSPage(IPaymentsApi paymentsApi, IUserLocalService usersServ, IUsersApi usersApi, ISubscriptionPaymentService applePay)
        {
            InitializeComponent();

            _paymentsApi = paymentsApi;
            _usersServ = usersServ;
            _userApi = usersApi;
            _applePay = applePay;
        }

        protected override async void OnAppearing()
        {
            try
            {
                AppLoader.HideAsync();

                var isZA = LocalStorageService.UserDetails.CountryCode == "+27";

                //PricePerDayZAR.IsVisible = true;
                
                string currency = "";
                (price, currency) = await _applePay.GetSubscriptionPrice();

                viewModel = new PaywallViewModel(MobileDeviceType.IOS);
                BindingContext = viewModel;

                viewModel.MonthlyPrice = $"{price}";
                viewModel.DailyPrice = "";
                SubscriptionAmount.Text = $"{price}/month";

                IsEligibleForFreeTrial = LocalStorageService.UserSubscription.Status == StatusType.Pending;
                CheckoutBtn.Text = IsEligibleForFreeTrial ? "START FREE TRIAL" : "PAY NOW";
            }
            catch(Exception ex)
            {
                viewModel.MonthlyPrice = "$5.99 USD";
            }
        }

        private async void ContinueToPaymentTapped(object sender, EventArgs e)
        {
#if IOS
            try
            {
                var purchaseStatus = await _applePay.PurchaseSubscription();
                if (purchaseStatus != StatusType.Success)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Payment", "Payment was not completed.", "OK"));
                    return;
                }

                // Get/refresh receipt and send to backend for validation + activation
                var receiptNSData = await _inAppPurchase.GetReceiptDataOrRefreshAsync(); // expose this helper or wrap it
                if (receiptNSData == null)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Receipt Error", "Could not retrieve receipt. Please try again.", "OK"));
                    return;
                }

                var receiptBase64 = Convert.ToBase64String(receiptNSData.ToArray());

                await AppLoader.ShowAsync("Activating subscription...");
                var amount = await _applePay.GetConvertedPriceForOvulaeZarOrUsd(price);
                amount = IsEligibleForFreeTrial ? 0 : amount;

                var res = await _paymentsApi.SubscriptionPaySuccessIOS(new PaymentSubscription
                {
                    Email = LocalStorageService.UserDetails.Email,
                    ReceiptBase64 = receiptBase64,
                    SubscriptionAmount = amount,
                    ProductId = "com.ovulae.org.ovulaeapp.monthlypremium.v3",
                    Platform = "ios"
                });

                await AppLoader.HideAsync();

                if (res.Success)
                {
                    await AppLoader.ShowAsync("Loading Dashboard...");
                    var dashboardPage = NavigationsHelper.GetDashboardPageNameFromModule(LocalStorageService.AppPrimaryGoal);
                    await Shell.Current.GoToAsync(dashboardPage);
                    await AppLoader.HideAsync();
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(
                        new BrandedAlertPopup("Subscription", "We couldn’t activate your subscription. Contact support.", "OK"));
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(
                    new BrandedAlertPopup("Error", "Something went wrong, please try again or contact support.", "Close"));
            }
#endif
        }

        private async void BackToLoginTapped(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(LoginPage));
                await Spinner.HideSpinnerAsync();
            }
            catch
            {
                
            }
        }

        private async void DeleteMyAccountTapped(object sender, EventArgs e)
        {
            try
            {
                var confirm = await YesNoModal.ShowYesNoModal("DELETE ACCOUNT?", "ARE YOU SURE YOU WANT TO DELETE YOUR ACCOUNT? ONCE DELETED, IT CANNOT BE REVERSED!");
                if (confirm == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Delelting your account...");
                    var deleteAccountResult = await _userApi.DeleteUserFullAccount(LocalStorageService.UserDetails.UserId);
                    await AppLoader.HideAsync();
                    if (deleteAccountResult.Success)
                    {
                        var popup = new BrandedAlertPopup("ACCOUNT DELETED", "Your account has been deleted. You will be logged out now and will not be able to login with same details unless you create a new account.");

                        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                        await popup.ShowWithResultAsync();

                        await _usersServ.Logout();

                        await AppLoader.ShowAsync("Logging out...");
                        await Shell.Current.GoToAsync(nameof(LoginPage));
                        await AppLoader.HideAsync();
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Deletion Failed", "Something went wrong whilst attempting to delete your data. Please try again, if the error persist please contact support@ovulae.com and request for your data to be deleted."));
                    }
                }
            }
            catch
            {

            }
        }

        private async void OpenAppleEula(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new("https://www.apple.com/legal/internet-services/itunes/dev/stdeula/");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Unable to open the link.", "OK");
            }
        }

        private async void OpenTermsOfUse(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new("https://ovulae.com/terms");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Unable to open the link.", "OK");
            }
        }

        private async void OpenPrivacyPolicy(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new("https://ovulae.com/privacy");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Unable to open the link.", "OK");
            }
        }
    }
}
