using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Status;
using OvulaeShared.Helpers.API;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Interface;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.Services.Payments.Paystack;

namespace OvulaeApp.Views.Subscription
{
    public partial class PaymentFailedPage : ContentPage
    {
        private readonly IPaystackApi _paystackApi;
        private readonly IWebInterfaceApiService _webApi;
        private readonly IUserLocalService _usersServ;
        private readonly IUsersApi _userApi;

        public PaymentFailedPage(IPaystackApi paystackApi, IUserLocalService usersServ, IUsersApi userApi)
        {
            InitializeComponent();

            _paystackApi = paystackApi;
            _webApi = new WebInterfaceApiService(OvulaeApiEndPoints.BASE_ADDRESS);
            _usersServ = usersServ;
            _userApi = userApi;
        }

        protected override void OnAppearing()
        {
            AppLoader.HideAsync();
        }

        private async void RetryPaymentTapped(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Retrying payment...");

                var apiPayload = new SecureApiRequest
                {
                    UserId = LocalStorageService.UserDetails.UserId,
                    Email = LocalStorageService.UserDetails.Email
                };

                var response = await _webApi.PostDataObject<GenericResult>("Payments/ChargeRecurringAsync", apiPayload);

                await AppLoader.HideAsync();

                if (response != null && response.Success)
                {
                    await _usersServ.ReloadUserData();

                    var popup = new BrandedAlertPopup("Success", "Payment retry was successfully processed, now navigating you to dashboard", "Ok");
                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                    await popup.ShowWithResultAsync();

                    await AppLoader.ShowAsync("Loading dashboard...");
                    await Shell.Current.GoToAsync(NavigationsHelper.GetDashboardPageName(LocalStorageService.UserCycleProfile.OvulaePrimaryGoal));
                    await AppLoader.HideAsync();
                }
                else 
                {
                    UpdateCardDetailsBtn.IsVisible = true;
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Retry Failed", "Retry payment failed. Please try again, if it still does not work please update your card payment details."));
                }
            }
            catch(Exception ex)
            {
                UpdateCardDetailsBtn.IsVisible = true;
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Retry Failed", "Retry payment failed. Please try again, if it still does not work please update your card payment details."));
            }
        }

        private async void UpdateCardDetailsPaymentTapped(object sender, EventArgs e)
        {
            try
            {
                var isZA = LocalStorageService.UserDetails.CountryCode == "+27";
                var amount = await PricingHelperFunctions.GetAmountFromSubscriptionTypeInZar(LocalStorageService.UserSubscription.SubscriptionType, isZA);

                if (!isZA)
                {
                    var rate = await PricingHelperFunctions.GetUsdZarRateAsync();
                    var amountZAR = rate.Value * amount;

                    var confirm = await CurrencyExchangeModal.ShowExchangeRateModal(amount, amountZAR);


                    if (confirm != ModalCloseType.Accept)
                        return;
                }

                await AppLoader.ShowAsync("Processing payment...");

                amount = amount * 100;

                var result = await _paystackApi.InitializeTransactionAsync(LocalStorageService.UserDetails.Email, (int)amount, "");

                if (result.Status)
                {
                    LocalStorageService.UserSubscription.LastPaymentReference = result.Data.Reference;
                    await _usersServ.UpdateUserSubscription();

                    var paymentUrl = result.Data.AuthorizationUrl;

                    var uri = new Uri(paymentUrl);
                    var options = new BrowserLaunchOptions
                    {
                        LaunchMode = BrowserLaunchMode.SystemPreferred,
                        PreferredToolbarColor = OvulaeColors.COLOR.ThemeClrMain,
                        PreferredControlColor = OvulaeColors.COLOR.ThemeLightGray,
                        TitleMode = BrowserTitleMode.Show,
                        Flags = BrowserLaunchFlags.PresentAsFormSheet // Optional: platform-specific
                    };

                    Preferences.Set("IsProcessingPayment", true);

                    await Browser.OpenAsync(uri, options);

                    await AppLoader.HideAsync();
                    await AppLoader.ShowAsync("Verifying payment...");
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", result.Message, "OK"));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"An error occurred: {ex.Message}", "OK"));
            }
        }

        private async void ContactSupportClicked(object sender, EventArgs e)
        {
            try
            {
                string email = "support@ovulae.com";
                string subject = Uri.EscapeDataString("URGENT Payment Assistance");
                string body = Uri.EscapeDataString("Hi Ovulae Team,\n\nI need assistance regarding payment...");

                var mailtoUri = new Uri($"mailto:{email}?subject={subject}&body={body}");
                await Launcher.Default.OpenAsync(mailtoUri);
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Unable to open mail app.", "OK"));
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
    }

}
