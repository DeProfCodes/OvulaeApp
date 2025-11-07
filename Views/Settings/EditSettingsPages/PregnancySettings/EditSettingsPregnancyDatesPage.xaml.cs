using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.Settings.EditSettingsPages.PregnancySettings
{
    public partial class EditSettingsPregnancyDatesPage : ContentPage
    {
        private EditSettingsPregnancySettingsViewModel vm;

        private readonly IUserLocalService _usersServ;
        private ModalUpdateType ChangeType;

        public EditSettingsPregnancyDatesPage(IUserLocalService usersServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingsPregnancySettingsViewModel();
                BindingContext = vm;

                var lmp = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                LMPDate.SelectedDate = lmp;
                PregnancyWeek.Value = SharedCommonFunctions.GetCurrentPregnancyWeekFromLMP(lmp);
                PregnancyDueDate.SelectedDate = SharedCommonFunctions.GetPregnancyDueDateFromLMP(lmp);

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

        private void CloseEditForm()
        {
            FormBorder.IsVisible = false;

            LMPedit.IsVisible = false;
            PregnancyWeekEdit.IsVisible = false;
            PregnancyDueEdit.IsVisible = false;
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            CloseEditForm();
        }

        private void UpdateSingleItem(object sender, string param)
        {
            FormBorder.IsVisible = true;

            LMPedit.IsVisible = param == "LMP";
            PregnancyWeekEdit.IsVisible = param == "CurrentWeek";
            PregnancyDueEdit.IsVisible = param == "PregnancyDueDate";

            if (param == "LMP") ChangeType = ModalUpdateType.LMP;
            if (param == "CurrentWeek") ChangeType = ModalUpdateType.CurrentWeekAlong;
            if (param == "PregnancyDueDate") ChangeType = ModalUpdateType.PregnancyDueDate;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (ChangeType == ModalUpdateType.CurrentWeekAlong && PregnancyWeek.Value <= 0)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Inputs", "Please enter valid pregnancy week."));

                    return;
                }

                DateTime calculatedLMP = DateTime.Now;

                if (ChangeType == ModalUpdateType.LMP) calculatedLMP = LMPDate.SelectedDate;
                if (ChangeType == ModalUpdateType.CurrentWeekAlong) calculatedLMP = DateTime.Today.AddDays(-(PregnancyWeek.Value - 1) * 7);
                if (ChangeType == ModalUpdateType.PregnancyDueDate) calculatedLMP = PregnancyDueDate.SelectedDate.AddDays(-280);
                
                var saveQuestion = await YesNoModal.ShowYesNoModal("Save Changes?", "Are you sure you want to update your settings?");
                if (saveQuestion == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Saving your changes...");

                    LocalStorageService.UserCycleProfile.LastPeriodDate = calculatedLMP;
                    var updateRes = await _usersServ.UpdateUserCycleProfile();

                    await AppLoader.HideAsync();

                    if (updateRes)
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your changes have been saved", "Ok"));
                        vm.UpdatePregnancyDetails(LocalStorageService.UserCycleProfile.LastPeriodDate.Value, true);
                        CloseEditForm();
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
