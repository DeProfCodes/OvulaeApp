using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Users;

namespace OvulaeApp.Views.Dashboard
{
    public partial class PartnerSharingPage : ContentPage
    {
        private readonly IUsersApi _userApi;
        private IUserLocalService _usersServ;

        private PartnerSharingViewModel vm;
        public PartnerSharingPage(IUsersApi userApi, IUserLocalService usersServ)
        {
            InitializeComponent();

            _userApi = userApi;
            _usersServ = usersServ;
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

            vm = new PartnerSharingViewModel();
            BindingContext = vm;

            base.OnAppearing();
        }

        private async void ShareWithYourPartnerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                var partnerShareRes = await PartnerSharing.ShowPartnerSharingModal(ModuleType.Ovulation, _userApi);
                if (partnerShareRes == ModalCloseType.Accept)
                {
                    await _usersServ.SaveLocalData();
                    vm.ReloadSharingInformation();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SavePermissionsButton(object sender, TappedEventArgs e)
        {

        }

        private async void OnRevokePartnerClicked(object sender, TappedEventArgs e)
        {
            try
            {
                var revokeUserQ = await YesNoModal.ShowYesNoModal("Revoke Partner Access", "Are you sure you want to revoke your partner's access? They won't be able to login afterwards.");
                if (revokeUserQ == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Revoking partner, please wait...");

                    var revokeStatus = await _userApi.RevokePartnerSharing(new SecureApiRequest { Email = LocalStorageService.PartnerDetails.Email });

                    await AppLoader.HideAsync();
                    if (revokeStatus.Success)
                    {
                        LocalStorageService.PartnerDetails.AccessStatus = AccountStatusType.Revoked;
                        LocalStorageService.PartnerDetails.LastUpdateDate = DateTime.UtcNow;

                        await _usersServ.SaveLocalData();
                        vm.ReloadSharingInformation();

                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Partner Revoked", "You have successfully revoked your partner's access, they won't be able to login from now on."));
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to revoke your partner's access, please try again later."));
                    }
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to revoke your partner's access, please try again later."));
            }
        }

        private async void OnReActivatePartnerClicked(object sender, TappedEventArgs e)
        {
            try
            {
                var reActivatePartnerQ = await YesNoModal.ShowYesNoModal("Re-activate Partner Access", "Are you sure you want to re-activate your partner's access? They will now have access to your tracking experience.");
                if (reActivatePartnerQ == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Re-activating partner, please wait...");

                    var reActivateStatus = await _userApi.ReActivatePartnerSharing(new SecureApiRequest { Email = LocalStorageService.PartnerDetails.Email });

                    await AppLoader.HideAsync();
                    if (reActivateStatus.Success)
                    {
                        LocalStorageService.PartnerDetails.AccessStatus = AccountStatusType.Active;
                        LocalStorageService.PartnerDetails.LastUpdateDate = DateTime.UtcNow;

                        await _usersServ.SaveLocalData();
                        vm.ReloadSharingInformation();

                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Partner Re-Activated", "You have successfully re-activated your partner's access, they will now be able to login and have access to your tracking experience."));
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to re-activate your partner's access, please try again later."));
                    }
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to re-activate your partner's access, please try again later."));
            }
        }

        private async void OnDeletePartnerClicked(object sender, TappedEventArgs e)
        {
            try
            {
                var deletePartnerQ = await YesNoModal.ShowYesNoModal("Delete Partner", "Are you sure you want to completely delete your partner's access? You can always invite them again.");
                if (deletePartnerQ == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Deleting partner, please wait...");

                    var deleteStatus = await _userApi.DeletePartnerSharing(new SecureApiRequest { Email = LocalStorageService.PartnerDetails.Email });

                    await AppLoader.HideAsync();
                    if (deleteStatus.Success)
                    {
                        LocalStorageService.PartnerDetails.AccessStatus = AccountStatusType.Deleted;
                        LocalStorageService.PartnerDetails.LastUpdateDate = DateTime.UtcNow;

                        await _usersServ.SaveLocalData();
                        vm.ReloadSharingInformation();

                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Partner Deleted", "You have successfully deleted your partner's access, they won't be able to login from now on."));
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to delete your partner's access, please try again later."));
                    }
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, Failed to delete your partner's access, please try again later."));
            }
        }
    }
}
