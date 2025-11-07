using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Authentication;

namespace OvulaeApp.Views.Settings
{
    public partial class SettingsPage : ContentPage
    {
        private readonly IUserLocalService _usersServ;
        private SettingsViewModel vm;

        public SettingsPage(IUserLocalService usersServ)
        {
            InitializeComponent();
            
            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                vm = new SettingsViewModel(Spinner);

                BindingContext = vm;
                base.OnAppearing();
            }
            catch
            {
                
            }
        }

        private async void LogoutTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Logging out...");

                await _usersServ.Logout();
                await Shell.Current.GoToAsync(nameof(LoginPage));

                await AppLoader.HideAsync();
            }
            catch
            {
                
            }
        }
    }
}
