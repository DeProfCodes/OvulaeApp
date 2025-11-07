using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Dashboard;

namespace OvulaeApp.Views.Dashboard
{
    public partial class NotificationsSettingsPage : ContentPage
    {
        public NotificationsSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoModal, MenopauseTrackerOnBoard,
                PregnancyComplete
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            base.OnAppearing();
        }
    }
}
