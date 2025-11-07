using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings
{
    public partial class EditSettingsHealthInfoPage : ContentPage
    {
        private EditSettingsUserHealthViewModel vm;

        private readonly IUserLocalService _usersServ;
        private ModalUpdateType ChangeType;

        public EditSettingsHealthInfoPage(IUserLocalService usersServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingsUserHealthViewModel();
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

        private async void EditDetailsButtonTapped(object sender, EventArgs e)
        {
            var isOpening = EditDetailsBtn.Text.ToLower().Contains("edit details");

            EditDetailsBtn.Text = isOpening ? "Close Editor" : "Edit Details";
            FormBorder.IsVisible = isOpening;

            WeightEdit.IsVisible = isOpening;
            HeightEdit.IsVisible = isOpening;
            BloodTypeEdit.IsVisible = isOpening;

            ChangeType = isOpening ? ModalUpdateType.All : ModalUpdateType.None;
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            EditDetailsBtn.Text = "Edit Details";
            FormBorder.IsVisible = false;

            WeightEdit.IsVisible = false;
            HeightEdit.IsVisible = false;
            BloodTypeEdit.IsVisible = false;
        }

        private void UpdateSingleItem(object sender, string param)
        {
            EditDetailsBtn.Text = "Close Editor";
            FormBorder.IsVisible = true;

            WeightEdit.IsVisible = param == "WeightUpdate";
            HeightEdit.IsVisible = param == "HeightUpdate";
            BloodTypeEdit.IsVisible = param == "BloodType";

            if (param == "WeightUpdate") ChangeType = ModalUpdateType.WeightUpdate;
            if (param == "HeightUpdate") ChangeType = ModalUpdateType.HeightUpdate;
            if (param == "BloodType") ChangeType = ModalUpdateType.BloodTypeUpdate;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var isAllUpdate = ChangeType == ModalUpdateType.All;

                if (WeightValue.Value <= 0 || HeightValue.Value <= 0 || WeightUnit.ActiveCategory == "" || HeightUnit.ActiveCategory == "" || BloodTypeOption.SelectedIndex < 0)
                {
                    var message = "Please enter valid";

                    if ((isAllUpdate || ChangeType == ModalUpdateType.WeightUpdate) && WeightValue.Value <= 0) message += " Weight Value,";
                    if ((isAllUpdate || ChangeType == ModalUpdateType.WeightUpdate) && WeightUnit.ActiveCategory == "") message += " Weight Unit,";
                    if ((isAllUpdate || ChangeType == ModalUpdateType.HeightUpdate) && HeightValue.Value <= 0) message += " Height Value,";
                    if ((isAllUpdate || ChangeType == ModalUpdateType.HeightUpdate) && HeightUnit.ActiveCategory == "") message += " Height Unit,";
                    if ((isAllUpdate || ChangeType == ModalUpdateType.BloodTypeUpdate) && BloodTypeOption.SelectedIndex < 0) message += " Blood Type,";

                    message = message.Substring(0, message.Length - 1);

                    if (message != "Please enter valid")
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Entry", message, "back"));
                        return;
                    }
                }

                var saveQuestion = await YesNoModal.ShowYesNoModal("Save Changes?", "Are you sure you want to update your settings?");
                if (saveQuestion == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Saving changes...");

                    if (isAllUpdate || ChangeType == ModalUpdateType.WeightUpdate)
                    {
                        LocalStorageService.UserBodyMetrics.Weight = WeightValue.Value;
                        LocalStorageService.UserBodyMetrics.WeightUnit = WeightUnit.ActiveCategory;
                    }
                    else if (isAllUpdate || ChangeType == ModalUpdateType.HeightUpdate)
                    {
                        LocalStorageService.UserBodyMetrics.Height = HeightValue.Value;
                        LocalStorageService.UserBodyMetrics.HeightUnit = HeightUnit.ActiveCategory;
                    }
                    else if (isAllUpdate || ChangeType == ModalUpdateType.BloodTypeUpdate)
                    {
                        if (BloodTypeOption.SelectedIndex >= 0)
                        {
                            var bloodType = BloodTypeOption.SelectedValue;
                            LocalStorageService.UserBodyMetrics.BloodGroup = bloodType.Substring(0, bloodType.Length - 1);
                            LocalStorageService.UserBodyMetrics.RhFactor = $"{bloodType[bloodType.Length - 1]}";
                        }
                        else
                        {
                            LocalStorageService.UserBodyMetrics.BloodGroup = "";
                            LocalStorageService.UserBodyMetrics.RhFactor = "";
                        }
                    }
                    
                    var updateRes = await _usersServ.UpdateUserBodyMetrics();

                    await AppLoader.HideAsync();

                    if (updateRes)
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your changes have been saved", "Ok"));
                        vm.UpdateHealthInformation(LocalStorageService.UserBodyMetrics, ChangeType, true);
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Your changes were not saved, please try again.", "Ok"));
                    }
                    ChangeType = ModalUpdateType.None;
                    vm.SaveButtonEnabled = false;
                }
            }
            catch
            {

            }
        }
    }
}
