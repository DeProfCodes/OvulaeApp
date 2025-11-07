using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.PrivacyPolicy;
using OvulaeApp.Views.Settings;
using OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings;
using OvulaeShared.Enums;
using OvulaeShared.Enums.User;

namespace OvulaeApp.Views.Dashboard
{
    public partial class DashboardProfilePage : ContentPage
    {
        private UserProfileViewModel viewModel;
        private IUserLocalService _usersServ;
        private ICycleService _cycleServ;

        public DashboardProfilePage(IUserLocalService usersServ, ICycleService cycleServ)
        {
            InitializeComponent();
            BaseTabs.SetSpinnerLoader(Spinner);
            
            _usersServ = usersServ;
            _cycleServ = cycleServ;
        }

        protected override void OnAppearing()
        {
            LoadUserProfileData();

            base.OnAppearing();
        }

        private void LoadUserProfileData()
        {
            try
            {
                BindingContext = new UserProfileViewModel(_cycleServ);

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
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to load user data: {ex.Message}");
                Shell.Current.CurrentPage.ShowPopup(new BrandedAlertPopup("Error", "Something went wrong while loading user data.", "OK"));
            }
        }

        private async void LogoutButtonTapped(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Logging you out...");

                await _usersServ.Logout();

                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                // Log the error or report it
                Console.WriteLine($"Logout failed: {ex.Message}");

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong while logging you out.", "OK"));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private async void OnPrivacyTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(PrivacyPolicyPage));

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Privacy policy page open failed: {ex.Message}");

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong while opening privacy.", "OK"));
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void OnTermsTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(TermsOfUsePage));

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terms page opening failed: {ex.Message}");

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong while opening terms page.", "OK"));
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void ProfileSharingTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync(NavigationsHelper.GetLoaderMessage(PageNameTypes.PartnerSharingPage));

                await Shell.Current.GoToAsync(NavigationsHelper.GetCommonAppPagePage(PageNameTypes.PartnerSharingPage));

                await AppLoader.HideAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Partner sharing page opening failed: {ex.Message}");

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong while opening partner sharing page.", "OK"));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private async void SettingsTapped(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync(NavigationsHelper.GetLoaderMessage(PageNameTypes.SettingsPage));

                await Shell.Current.GoToAsync(nameof(SettingsPage));

                await AppLoader.HideAsync();
            }
            catch
            {
                await AppLoader.HideAsync();
            }
        }

        private async void EditPersonalDetailsTapped(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Opening edit personal details...");

                await Shell.Current.GoToAsync(nameof(EditSettingsPersonalInfoPage));

                await AppLoader.HideAsync();
            }
            catch
            {
                await AppLoader.HideAsync();
            }
        }
    }
}
