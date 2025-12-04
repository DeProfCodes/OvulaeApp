using CommunityToolkit.Maui.Views;
using OvulaeApp.Services.Notifications;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    public partial class MenopauseNotificationsPage : ContentPage
    {
        private MenopauseNotificationsViewModel vm;

        public MenopauseNotificationsPage()
        {
            InitializeComponent();

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

            vm = new MenopauseNotificationsViewModel();
            BindingContext = vm;

            base.OnAppearing();
        }

        private async void SaveNotificationsPreferences(object sender, TappedEventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Saving your preferences...");

                vm.Notifications.UseOwnTime = vm.ShowManualTime;
                //var success = await _notificationsPrefServ.UpdateMenopauseNotifications(vm.Notifications);

                if (true)
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
