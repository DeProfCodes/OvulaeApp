using CommunityToolkit.Maui.Views;
using OvulaeApp.Services.Notifications;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    public partial class MenopauseNotificationsPage : ContentPage
    {
        private readonly INotificationsPreferenceService _notificationsPrefServ;
        private readonly INotificationSchedulerService _notificationsSendServ;

        private MenopauseNotificationsViewModel vm;

        public MenopauseNotificationsPage(INotificationsPreferenceService notificationsPrefServ, INotificationSchedulerService notificationsSendServ)
        {
            InitializeComponent();

            _notificationsPrefServ = notificationsPrefServ;
            _notificationsSendServ = notificationsSendServ;
        }

        protected override void OnAppearing()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            vm = new MenopauseNotificationsViewModel(_notificationsPrefServ);
            BindingContext = vm;

            base.OnAppearing();
        }

        private async void SaveNotificationsPreferences(object sender, TappedEventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Saving your preferences...");

                vm.Notifications.UseOwnTime = vm.ShowManualTime;
                var success = await _notificationsPrefServ.UpdateMenopauseNotifications(vm.Notifications);

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
