using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.Subscription;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Subscription;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Payments.Paystack;

namespace OvulaeApp.Views.Subscription
{
    public partial class PaymentWallPage : ContentPage
    {
        private readonly IPaystackApi _paystackApi;
        private readonly IUserLocalService _usersServ;
        private readonly IUsersApi _userApi;
        private readonly ISubscriptionService _subscriptionServ;

        private PaywallViewModel viewModel;

        private bool _isHandlingCheckChange = false;

        public PaymentWallPage(IPaystackApi paystackApi, IUserLocalService usersServ, IUsersApi usersApi, ISubscriptionService subscriptionServ)
        {
            InitializeComponent();

            _paystackApi = paystackApi;
            _usersServ = usersServ;
            _userApi = usersApi;
            _subscriptionServ = subscriptionServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                AppLoader.HideAsync();

                var isZA = LocalStorageService.UserDetails.CountryCode == "+27";

                viewModel = new PaywallViewModel(MobileDeviceType.Android);
                BindingContext = viewModel;
            }
            catch
            {
                
            }
        }

        private async void ContinueToPaymentTapped(object sender, EventArgs e)
        {
            try
            {
                var isZA = LocalStorageService.UserDetails.CountryCode == "+27";
                var isFreeTrial = false;// LocalStorageService.UserSubscription.Status == StatusType.Pending && FreeTrialOption.IsChecked;

                double amount = isFreeTrial ? 0.06 : 5.99;

                var rate = await PricingHelperFunctions.GetUsdZarRateAsync();
                double amountZAR = isFreeTrial ? (isZA ? 0.50 : rate.Value * amount) : (isZA ? 95 : rate.Value * amount);

                if (!isZA)
                {
                    var confirm = await CurrencyExchangeModal.ShowExchangeRateModal(amount, amountZAR);
                    if (confirm != ModalCloseType.Accept) return;
                }

                amount = amountZAR;

                await AppLoader.ShowAsync("Processing payment...");

                var cents = (int)Math.Round(amount * 100);
                var init = await _paystackApi.InitializeTransactionAsync(LocalStorageService.UserDetails.Email, cents, "");

                if (init == null || !init.Status || init.Data == null)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Could not start payment.", "OK"));
                    return;
                }

                LocalStorageService.UserSubscription.LastPaymentReference = init.Data.Reference;
                await _usersServ.UpdateUserSubscription();

                await SecureStorage.SetAsync("pending_paystack_ref", init.Data.Reference);
                Preferences.Set("IsProcessingPayment", true);

                var start = new Uri(init.Data.AuthorizationUrl);      // Paystack checkout
                var end = new Uri("ovulae://paystack/callback");    // deep link

                try
                {
                    var authResult = await WebAuthenticator.AuthenticateAsync(start, end);
                    if (authResult?.Properties.TryGetValue("reference", out var r) == true && !string.IsNullOrEmpty(r))
                        await SecureStorage.SetAsync("pending_paystack_ref", r);
                }
                catch (OperationCanceledException) { /* user canceled; still verify */ }
                catch (Exception ex)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"Auth error: {ex.Message}", "OK"));
                }

                await AppLoader.HideAsync();
                await AppLoader.ShowAsync("Verifying payment...");
                await VerifyPaymentAfterResumeSafe();
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"An error occurred: {ex.Message}", "OK"));
            }
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

        private void FreeiumBorderClicked(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Border brd)
                {
                    if (brd == BorderPremium)
                    {
                        PremiumUSDOption.CheckUncheck(true);
                    }
                    else if (brd == BorderFreeTrial)
                    {
                        FreeTrialOption.CheckUncheck(true);
                    }
                }
            }
            catch
            {
                
            }
        }

        private void PremiumCheckedChanged(object sender, bool e)
        {
            if (_isHandlingCheckChange) return;

            try
            {
                _isHandlingCheckChange = true;

                BorderPremium.Background = OvulaeColors.BRUSH.BrushThemeClrPurple;
                BorderFreeTrial.Background = Brush.Transparent;
                FreeTrialOption.CheckUncheck(false);
            }
            finally
            {
                _isHandlingCheckChange = false;
            }
        }

        private void FreeTrialCheckedChanged(object sender, bool e)
        {
            if (_isHandlingCheckChange) return;

            try
            {
                _isHandlingCheckChange = true;

                BorderFreeTrial.Background = OvulaeColors.BRUSH.BrushThemeClrPurple;
                BorderPremium.Background = Brush.Transparent;
                PremiumUSDOption.CheckUncheck(false);
            }
            finally
            {
                _isHandlingCheckChange = false;
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
