using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Notifications;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.Notifications;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Splashscreen;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Models.App;
using OvulaeShared.Services.APIs.App;

namespace OvulaeApp
{
    public partial class MainPage : ContentPage
    {
        private ILocalDbService _localDbServ => ServiceHelper.GetService<ILocalDbService>();
        private IOneSignalNotificationService _oneSignalService => ServiceHelper.GetService<IOneSignalNotificationService>();
        private IAppService _appService => ServiceHelper.GetService<IAppService>();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CheckAppVersionAsync();
            await NavigateToStartupPage();
        }

        private async Task CheckAppVersionAsync()
        {
            try
            {
                var version = AppInfo.VersionString;
                var build = Convert.ToInt32(AppInfo.BuildString);
                var platform = DeviceInfo.Platform == DevicePlatform.Android ? MobileDeviceType.Android : MobileDeviceType.IOS;
                
                var response = await _appService.VersionCheck(version, build, platform);

                if (response.IsUpdateRequired)
                {
                    await ShowForceUpdateDialog(response);
                }
            }
            catch (Exception ex)
            {
                // Optional: allow app to continue if API fails
                Console.WriteLine(ex);
            }
        }

        private async Task ShowForceUpdateDialog(AppVersionCheckResponse response)
        {
            var updateAppPopup = new BrandedAlertPopup("Update Required", response.Message, "Update Now!");

            await Shell.Current.CurrentPage.ShowPopupAsync(updateAppPopup);

            var update = await updateAppPopup.ShowWithResultAsync();

            if (update)
            {
                await Launcher.Default.OpenAsync(response.StoreUrl);
                Application.Current.Quit();
            }
        }

        private async Task NavigateToStartupPage()
        {
            try
            {
                var loadDataStatus = await _localDbServ.LoadStartupData();

                //Cannot load data | Splash
                if (!loadDataStatus)
                {
                    await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
                    return;
                }

                var subscription = LocalStorageService.UserSubscription;
                var userDetails = LocalStorageService.UserDetails;

                // No user details loaded | Splash | Login
                if (userDetails == null || string.IsNullOrEmpty(userDetails.UserId))
                {
                    var page = (subscription == null || subscription.Id == 0) ? nameof(SplashWelcomePage) : nameof(LoginPage);
                    await Shell.Current.GoToAsync(page);
                    return;
                }

                // No subscription found | Splash
                if (subscription == null)
                {
                    await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
                    return;
                }

                var isActiveSubscription = subscription.Status == StatusType.Active && subscription.NextPaymentDate > DateTime.Now;
                var isFreeTrial = subscription.Status == StatusType.Open && subscription.NextPaymentDate > DateTime.Now;

                // Expired subscription & no free trial | Login
                if (!isActiveSubscription && !isFreeTrial)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                    return;
                }

                LocalStorageService.Authenticated = true;
                await _oneSignalService.InitializeOneSignal();

                // App booting from notification tap | DayLogger
                var pending = PendingNavigationCache.Consume();
                if (pending != null)
                {
                    var route = NavigationsHelper.GetDayLogPageNameFromModuleName(pending.Value.module);
                    await Shell.Current.GoToAsync($"{route}?entryId={pending.Value.entryId}");
                    return;
                }

                // Normal app load | Dashboard
                var dashboardPage = NavigationsHelper.GetDashboardPageNameFromModule(LocalStorageService.AppPrimaryGoal);
                await Shell.Current.GoToAsync(dashboardPage);

            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
            }
        }
    }
}
