using CommunityToolkit.Maui.Views;
using OvulaeApp.Services.Notifications;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    public partial class PregnancyNotificationsPage : ContentPage
    {
        private PregnancyNotificationsViewModel vm;

        public bool _isNavigating { get; set; }

        public PregnancyNotificationsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            vm = new PregnancyNotificationsViewModel();
            BindingContext = vm;

            base.OnAppearing();
        }

        private async void SaveNotificationsPreferences(object sender, TappedEventArgs e)
        {
            if (_isNavigating && vm.IsBusy)
                return;

            vm.IsBusy = true;
            _isNavigating = true;

            try
            {
                await AppLoader.ShowAsync("Saving your preferences...");

                vm.Notifications.UseOwnTime = vm.ShowManualTime;
                var success = true;// await _notificationsPrefServ.UpdatePregnancyNotifications(vm.Notifications);

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
            finally
            {
                await AppLoader.HideAsync();
                vm.IsBusy = false;
                _isNavigating = false;
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
