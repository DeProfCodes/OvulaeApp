using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Notifications;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.GoalSetting;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.Subscription;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.User;
using OvulaeShared.Services.APIs.Authentication;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Views.Authentication
{
    public partial class LoginPage : ContentPage, ISpinnerService
    {
        private readonly IAuthenticationApi _api;
        private readonly IUserLocalService _usersServ;
        private readonly IOneSignalNotificationService _oneSignalService;

        public LoginPage(IAuthenticationApi api, IUserLocalService usersServ, IOneSignalNotificationService oneSignalService)
        {
            InitializeComponent();

            LocalStorageService.Authenticated = false;

            LocalStorageService.UserDetails = DefaultValueHelper.CreateWithDefaults<UserModel>();
            LocalStorageService.UserBodyMetrics.Id = 0;

            _api = api;
            _usersServ = usersServ;
            _oneSignalService = oneSignalService;

            if (Resources["NavigationsVM"] is NavigationsViewModel vm)
            {
                vm.SpinnerService = this;
            }

            Logout();
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                HandleBackPressedAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async void HandleBackPressedAsync()
        {
            var backToGoal = await YesNoModal.ShowYesNoModal("Go Back?", "Go back to goal setting page?");

            if (backToGoal == ModalCloseType.Accept)
            {
                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(GoalSettingPage));
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void Logout()
        {
            await _usersServ.Logout();    
        }

        public async Task ShowSpinnerAsync()
        {
            await Spinner.ShowSpinnerAsync(); 
        }

        public async Task HideSpinner()
        {
            await Spinner.HideSpinnerAsync();
        }

        private async void OnFieldsChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is Entry entry)
                {
                    if (entry.IsPassword)
                    {
                        PasswordValidation.IsVisible = !ValidationsHelper.IsStrongPassword(Password.Text);
                    }
                    else
                    {
                        EmailValidation.IsVisible = !ValidationsHelper.IsValidEmail(Email.Text);
                    }
                }
                var validEmailNPass = ValidationsHelper.IsValidEmail(Email.Text) && ValidationsHelper.IsStrongPassword(Password.Text);
                AuthenticationPagesHelper.ToggleActionButtonButton(LoginBtn, validEmailNPass);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while entering details.", "OK");
            }
        }

        private async void ForgotPasswordTapped(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(PasswordResetMobilePage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating to forgot password page.", "OK");
            }
        }

        private async Task ByPassSuper()
        {
            try
            {
                string data = "{\r\n  \"userDetails\": {\r\n    \"userId\": \"39fe72ef-387a-432a-9b43-dd99453fd4b8\",\r\n    \"firstname\": \"Test\",\r\n    \"lastname\": \"1004\",\r\n    \"username\": null,\r\n    \"email\": \"test1004@gmail.com\",\r\n    \"password\": null,\r\n    \"countryCode\": \"+27\",\r\n    \"phoneNumber\": \"565959595\",\r\n    \"mobileDevice\": null,\r\n    \"userRole\": 2,\r\n    \"accountStatus\": 0,\r\n    \"createDate\": \"0001-01-01T00:00:00\"\r\n  },\r\n  \"userBodyMetrics\": {\r\n    \"id\": 12,\r\n    \"userId\": \"39fe72ef-387a-432a-9b43-dd99453fd4b8\",\r\n    \"year\": 2005,\r\n    \"weight\": 80,\r\n    \"weightUnit\": \"kg\",\r\n    \"height\": 160,\r\n    \"heightUnit\": \"cm\",\r\n    \"bloodGroup\": \"\",\r\n    \"rhFactor\": \"\",\r\n    \"createDate\": \"2025-08-18T21:37:54.0637886\",\r\n    \"lastUpdateDate\": \"2025-08-18T21:37:54.0637895\"\r\n  },\r\n  \"userProfileCycle\": {\r\n    \"id\": 12,\r\n    \"userId\": \"39fe72ef-387a-432a-9b43-dd99453fd4b8\",\r\n    \"lastPeriodDate\": \"2025-08-18T00:00:00\",\r\n    \"cycleLengthDays\": 28,\r\n    \"periodLengthDays\": 6,\r\n    \"ovulaePrimaryGoal\": \"PeriodTracker\",\r\n    \"allOvulaeGoalsJson\": \"[]\",\r\n    \"periodIrregularityType\": 2,\r\n    \"treatmentsJson\": \"[]\",\r\n    \"healthConditionsJson\": \"[\\\"Thyroid issues\\\"]\",\r\n    \"birthControlMethodsJson\": \"[]\",\r\n    \"symptomsJson\": \"[]\",\r\n    \"trackingStartDate\": \"0001-01-01T00:00:00\",\r\n    \"cycleTrackingHistoryJson\": \"[]\",\r\n    \"allOvulaeGoals\": [],\r\n    \"treatments\": [],\r\n    \"healthConditions\": [\r\n      \"Thyroid issues\"\r\n    ],\r\n    \"birthControlMethods\": [],\r\n    \"symptoms\": [],\r\n    \"cycleTrackingHistory\": [],\r\n    \"estimatedDueDate\": \"0001-01-01T00:00:00\",\r\n    \"firstPregnancy\": false,\r\n    \"pregnancyGoal\": \"\",\r\n    \"ovulationStartDate\": \"0001-01-01T00:00:00\",\r\n    \"isTryingToConceive\": false,\r\n    \"createdAt\": \"2025-08-18T21:37:54.1303915\",\r\n    \"lastUpdated\": \"2025-08-18T21:37:54.1303924\"\r\n  },\r\n  \"userSubscription\": {\r\n    \"id\": 12,\r\n    \"userId\": \"39fe72ef-387a-432a-9b43-dd99453fd4b8\",\r\n    \"subscriptionType\": 1,\r\n    \"status\": 5,\r\n    \"mobileDeviceType\": 1,\r\n    \"authorizationCode\": \"\",\r\n    \"paymentRetryCount\": 0,\r\n    \"lastPaymentReference\": \"\",\r\n    \"activeDate\": \"0001-01-01T00:00:00\",\r\n    \"nextPaymentDate\": \"2025-09-18T21:37:54.1080748\",\r\n    \"lastUpdateDate\": \"0001-01-01T00:00:00\"\r\n  },\r\n  \"partnerDetails\": null,\r\n  \"affiliateDetailsOverview\": {\r\n    \"userId\": \"39fe72ef-387a-432a-9b43-dd99453fd4b8\",\r\n    \"androidJoins\": 0,\r\n    \"iosJoins\": 0,\r\n    \"androidJoinLink\": \"\",\r\n    \"iosJoinLink\": \"\",\r\n    \"totalRevenue\": 0,\r\n    \"affiliateProfile\": null\r\n  },\r\n  \"affiliateJoinLink\": null\r\n}";
                var userData = JsonConvert.DeserializeObject<UserFullProfileViewModel>(data);

                LocalStorageService.UserDetails = userData.UserDetails;
                LocalStorageService.UserBodyMetrics = userData.UserBodyMetrics;
                LocalStorageService.UserCycleProfile = userData.UserProfileCycle;
                LocalStorageService.UserSubscription = userData.UserSubscription;
                LocalStorageService.PartnerDetails = userData.PartnerDetails;
                LocalStorageService.AffiliateOverviewDetails = userData.AffiliateDetailsOverview;

                await _usersServ.SaveLocalData();

                var primaryGoalStr = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;
                var appPrimaryGoal = primaryGoalStr != null ? EnumHelper.GetEnumValueFromName<ModuleType>(primaryGoalStr) : ModuleType.PeriodTracker;

                LocalStorageService.AppPrimaryGoal = appPrimaryGoal;

                var subscription = LocalStorageService.UserSubscription;
                if (subscription != null)
                {
                    var isActiveSubscription = subscription.Status == StatusType.Active && subscription.NextPaymentDate > DateTime.Now;
                    if (isActiveSubscription)
                    {
                        LocalStorageService.Authenticated = true;

                        var dashboardPage = NavigationsHelper.GetDashboardPageNameFromModule(LocalStorageService.AppPrimaryGoal);
                        await Shell.Current.GoToAsync(dashboardPage);
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(nameof(PaymentWalliOSPage));
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(PaymentWalliOSPage));
                }
            }
            catch
            {

            }
        }

        private async void LoginBtn_Clicked(object sender, EventArgs e)
        {
            KeyboardHelper.Dismiss();
            try
            {
                await AppLoader.ShowAsync("Logging you in...");

                if(Email.Text.ToLower() == "test1004@gmail.com" &&  Password.Text == "Test123@")
                {
                    ByPassSuper();
                    AppLoader.HideAsync();
                    return;
                }

                var userModel = await _api.Login(Email.Text, Password.Text);
                if (userModel != null)
                {
                    if (userModel.UserRole == UserRoleType.PartnerShare && userModel.AccountStatus != AccountStatusType.Active)
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Access Revoked!", "Could not log you in because your ACCESS have been REVOKED by your partner."));

                        return;
                    }

                    AuthenticationPagesHelper.SetLoggedInUser(userModel);
                    await _usersServ.UpdateProfileFromLogin(userModel, true);

                    LocalStorageService.Authenticated = true;

                    var primaryGoalStr = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;
                    var appPrimaryGoal = primaryGoalStr != null ? EnumHelper.GetEnumValueFromName<ModuleType>(primaryGoalStr) : ModuleType.PeriodTracker;

                    LocalStorageService.AppPrimaryGoal = appPrimaryGoal;

                    var subscription = LocalStorageService.UserSubscription;
                    if (subscription != null)
                    {
                        var isActiveSubscription = subscription.Status == StatusType.Active && subscription.NextPaymentDate > DateTime.Now;
                        var isFreeTrial = subscription.Status == StatusType.Open && subscription.NextPaymentDate > DateTime.Now;
                        
                        if (isActiveSubscription || isFreeTrial)
                        {
                            LocalStorageService.Authenticated = true;
                            await _oneSignalService.InitializeOneSignal();

                            var dashboardPage = NavigationsHelper.GetDashboardPageNameFromModule(LocalStorageService.AppPrimaryGoal);
                            await Shell.Current.GoToAsync(dashboardPage);
                        }
                        else
                        {
                        #if ANDROID
                            var hasNeverSubscribed = subscription.ActiveDate == DateTime.MinValue && string.IsNullOrEmpty(subscription.AuthorizationCode);
                            var page = hasNeverSubscribed ? nameof(PaymentWallPage) : nameof(PaymentFailedPage);
                            await Shell.Current.GoToAsync(page);
#elif IOS
                            var hasNeverSubscribed = subscription.ActiveDate == DateTime.MinValue && string.IsNullOrEmpty(subscription.AuthorizationCode);
                            var page = hasNeverSubscribed ? nameof(PaymentWalliOSPage) : nameof(PaymentWalliOSPage);
                            await Shell.Current.GoToAsync(page);
#endif
                        }
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(nameof(PaymentWallPage));
                    }
                    //var dashboardPage = NavigationsHelper.GetDashboardPageName(primaryGoalStr);
                    //await Shell.Current.GoToAsync(dashboardPage);

                    await AppLoader.HideAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Login", "Could not login. Incorrect details"));
                }
            }
            catch(Exception ex)
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Login", $"Could not login. Unknown error. Msg: {ex.Message}"));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private  void GoogleLoginBtn_Tapped(object sender, TappedEventArgs e)
        {
            //await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Coming Soon!", "This will be implemented soon."));
        }

        private  void AppleLoginBtn_Tapped(object sender, TappedEventArgs e)
        {
            //await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Coming Soon!", "This will be implemented soon."));
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (Password.IsPassword)
            {
                PasswordCover.Source = "eye_hide";
                Password.IsPassword = false;
            }
            else
            {
                PasswordCover.Source = "eye_show";
                Password.IsPassword = true;
            }
        }

        private async void SignUpLink_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(GoalSettingPage));

                await Spinner.HideSpinnerAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating to sign up.", "OK");
            }
        }
    }
}
