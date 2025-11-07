using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Status;

namespace OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings
{
    public partial class EditSettingsMembershipInfoPage : ContentPage
    {
        private EditSettingsSubscriptionViewModel vm;

        private readonly IUserLocalService _usersServ;
        
        public EditSettingsMembershipInfoPage(IUserLocalService usersServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingsSubscriptionViewModel();
                BindingContext = vm;

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
            }
            catch (Exception ex)
            {

            }
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            VisualEventsHelper.TapDimEffectGray(BackBtnBorder);
            await Shell.Current.GoToAsync("..");
        }

        private async void UpdateMembershipStatus(object sender, EventArgs e)
        {
            try
            {
                var isActiveNow = LocalStorageService.UserSubscription.Status == StatusType.Active;
                var message = isActiveNow ? "CANCEL" : "RE-ACTIVATE";
                var saveQuestion = await YesNoModal.ShowYesNoModal("Save Changes?", $"Are you sure you want {message} your membership?");
                
                if (saveQuestion == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Saving your changes...");

                    var updateRes = await _usersServ.UpdateUserSubscription(isActiveNow ? StatusType.Cancelled : StatusType.Active);
                    if (updateRes.Success)
                    {
                        var statusMsg = isActiveNow ? "CANCELLED" : "RE-ACTIVATED";
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", $"Your subscription has been {statusMsg} successfully.", "Ok"));

                        vm.UpdateSubscriptionInformation(LocalStorageService.UserSubscription, true);
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Failed to update subscription status.", "Ok"));
                    }
                    await AppLoader.HideAsync();
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Failed to update subscription status.", "Ok"));
            }
        }
    }
}
