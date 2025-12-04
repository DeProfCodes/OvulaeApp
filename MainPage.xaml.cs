using System.Security.Cryptography;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services;
using OvulaeApp.Services.Jobs;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.Notifications;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.GoalSetting;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Onboarding;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Onboarding;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.Splashscreen;
using OvulaeApp.Views.Subscription;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;

namespace OvulaeApp
{
    public partial class MainPage : ContentPage
    {
        private ILocalDbService _localDbServ => ServiceHelper.GetService<ILocalDbService>();
        private IFcmNotificationService _fcmService => ServiceHelper.GetService<IFcmNotificationService>();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await NavigateToStartupPage();
        }

        private async Task NavigateToStartupPage()
        {
            try
            {
                var loadDataStatus = await _localDbServ.LoadStartupData();
               
                await _fcmService.InitializeFirebase();
                //await Shell.Current.GoToAsync(nameof(PregnancyDashboardDayLoggerPage));

                //return;

                //await Task.Delay(2000);

                if (loadDataStatus)
                {
                    var subscription = LocalStorageService.UserSubscription;
                    var userDetails = LocalStorageService.UserDetails;
                    if (userDetails != null && !string.IsNullOrEmpty(userDetails.UserId))
                    {
                        if (subscription == null)
                        {
                            await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
                        }
                        else 
                        {
                            var isActiveSubscription = subscription.Status == StatusType.Active && subscription.NextPaymentDate > DateTime.Now;
                            var isFreeTrial = subscription.Status == StatusType.Open && subscription.NextPaymentDate > DateTime.Now;

                            if (isActiveSubscription || isFreeTrial)
                            {
                                LocalStorageService.Authenticated = true;

                                var dashboardPage = NavigationsHelper.GetDashboardPageNameFromModule(LocalStorageService.AppPrimaryGoal);
                                await Shell.Current.GoToAsync(dashboardPage);
                            }
                            else
                            {
                                //var hasNeverSubscribed = subscription.ActiveDate == DateTime.MinValue && string.IsNullOrEmpty(subscription.AuthorizationCode);
                                //var page = hasNeverSubscribed ? nameof(PaymentWallPage) : nameof(PaymentFailedPage);
                                await Shell.Current.GoToAsync(nameof(LoginPage));
                            }
                        }
                    }
                    else
                    {
                        var page = (subscription == null || subscription.Id == 0) ? nameof(SplashWelcomePage) : nameof(LoginPage);
                        await Shell.Current.GoToAsync(page);
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
                }
            }
            catch(Exception ex) 
            {
                await Shell.Current.GoToAsync(nameof(SplashWelcomePage));
            }
        }
    }
}
