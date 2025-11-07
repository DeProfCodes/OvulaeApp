
using CommunityToolkit.Maui.Views;
using OvulaeApp.Services.Notifications;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using Plugin.LocalNotification;

namespace OvulaeApp.Views.PeriodTracker.Dashboard
{
    public partial class PeriodNotificationsPage : ContentPage
    {
        private readonly INotificationsPreferenceService _notificationsPrefServ;
        private readonly INotificationSchedulerService _notificationsSendServ;
        
        private PeriodOvulationNotificationsViewModel vm;
        
        public PeriodNotificationsPage(INotificationsPreferenceService notificationsPrefServ, INotificationSchedulerService notificationsSendServ)
        {
            InitializeComponent();

            _notificationsPrefServ = notificationsPrefServ;
            _notificationsSendServ = notificationsSendServ;
        }

        protected override void OnAppearing()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            vm = new PeriodOvulationNotificationsViewModel(_notificationsPrefServ);
            BindingContext = vm;

            base.OnAppearing();
        }

        private async void SaveNotificationsPreferences(object sender, TappedEventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Saving your preferences...");

                vm.Notifications.UseOwnTime = vm.ShowManualTime;
                var success = await _notificationsPrefServ.UpdatePeriodOvulationNotifications(vm.Notifications);

                if (success)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved!", "Your notifications preferences have been saved!", "Ok"));
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed!", "Your notifications were not saved, Please try again.", "Ok"));
                }

                await AppLoader.HideAsync();
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error!", "Your notifications were not saved, Please try again.", "Ok"));
                await AppLoader.HideAsync();
            }
        }

        private void ManualTimeToggle(object sender, EventArgs e)
        {
            if (sender is Views.Components.Controls.Switch s)
            {
                vm.ShowManualTime = s.IsToggled;
            }
        }
    }
}
