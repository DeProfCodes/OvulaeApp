using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings
{
    public partial class EditSettingsPersonalInfoPage : ContentPage
    {
        private EditSettingsPersonalInfoViewModel vm;
        private ModalUpdateType ChangeType;

        private readonly IUserLocalService _usersServ;
        
        public EditSettingsPersonalInfoPage(IUserLocalService usersServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingsPersonalInfoViewModel();
                BindingContext = vm;

                MyYearPicker.SetBinding(
                    YearPicker.SelectedYearProperty,
                    new Binding("SelectedYear", source: vm, mode: BindingMode.TwoWay)
                );

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

        private async void EditDetailsButtonTapped(object sender, EventArgs e)
        {
            var isOpening = EditDetailsBtn.Text.ToLower().Contains("edit details");

            EditDetailsBtn.Text = isOpening ? "Close Editor" : "Edit Details";
            FormBorder.IsVisible = isOpening;

            FirstnameEdit.IsVisible = isOpening;
            LastnameEdit.IsVisible = isOpening;
            BirthYearEdit.IsVisible = isOpening;

            if (!isOpening)
            {
                vm.FirstnameProp.Value = LocalStorageService.UserDetails.Firstname;
                vm.LastnameProp.Value = LocalStorageService.UserDetails.Lastname;
            }
            else
            {
                ChangeType = ModalUpdateType.All;
            }
        }

        private void CloseModalPage()
        {
            EditDetailsBtn.Text = "Edit Details";
            FormBorder.IsVisible = false;

            FirstnameEdit.IsVisible = false;
            LastnameEdit.IsVisible = false;
            BirthYearEdit.IsVisible = false;

            vm.FirstnameProp.Value = LocalStorageService.UserDetails.Firstname;
            vm.LastnameProp.Value = LocalStorageService.UserDetails.Lastname;
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            CloseModalPage();
        }

        private void UpdateSingleItem(object sender, string param)
        {
            EditDetailsBtn.Text = "Close Editor";
            FormBorder.IsVisible = true;

            FirstnameEdit.IsVisible = param == "Firstname";
            LastnameEdit.IsVisible = param == "Lastname";
            BirthYearEdit.IsVisible = param == "BirthYear";

            if (param == "Firstname") ChangeType = ModalUpdateType.FirstnameUpdate;
            if (param == "Lastname") ChangeType = ModalUpdateType.LastnameUpdate;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var isAllChange = ChangeType == ModalUpdateType.All;

                if (vm.FirstnameProp.Value == "" || vm.LastnameProp.Value == "")
                {
                    var message = vm.FirstnameProp.Value == "" ? "Please enter your firstname" : "";
                    var bothMissing = vm.LastnameProp.Value == "";
                    message += vm.LastnameProp.Value == "" ? (bothMissing ? " and enter your lastname." : "Please enter your lastname.") :".";

                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Entry", message, "back"));
                    return;
                }

                var saveQuestion = await YesNoModal.ShowYesNoModal("Save Changes?", "Are you sure you want to update your settings?");
                if (saveQuestion == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Saving your changes...");

                    ModalCloseType updateType = ModalCloseType.None;

                    if (isAllChange || ChangeType == ModalUpdateType.FirstnameUpdate)
                    {
                        LocalStorageService.UserDetails.Firstname = vm.FirstnameProp.Value;
                    }
                    else if (isAllChange || ChangeType == ModalUpdateType.LastnameUpdate)
                    {
                        LocalStorageService.UserDetails.Lastname = vm.LastnameProp.Value;
                    }

                    var updateViewModel = new UserDetailsUpdateViewModel
                    {
                        UserId = LocalStorageService.UserDetails.UserId,
                        Firstname = LocalStorageService.UserDetails.Firstname,
                        Lastname = LocalStorageService.UserDetails.Lastname,
                        BirthYear = vm.SelectedYear
                    };

                    var updateRes = await _usersServ.UpdateUserDetails(updateViewModel);

                    await AppLoader.HideAsync();

                    if (updateRes.Success)
                    {
                        CloseModalPage();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your changes have been saved", "Ok"));
                        vm.UpdatePersonalInformation(LocalStorageService.UserDetails, ChangeType, true);
                    }
                    else if (updateType == ModalCloseType.Reject)
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Your changes were not saved, please try again.", "Ok"));
                    }
                    vm.SaveButtonEnabled = false;
                }
            }
            catch
            {

            }
        }

        private async void SelectBirthYearTapped(object sender, EventArgs e)
        {
            await MyYearPicker.ShowAsync();
            vm.BirthYearProp.Update($"{vm.SelectedYear}");
            vm.SaveButtonEnabled = true;
        }
    }
}
