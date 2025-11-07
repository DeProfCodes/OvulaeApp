using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;

namespace OvulaeApp.Views.Settings.EditSettingsPages.OvulationSettings
{
    public partial class EditSettingsOvulationPage : ContentPage
    {
        private EditSettingPeriodSettingsViewModel vm;

        private readonly IUserLocalService _usersServ;
        
        public EditSettingsOvulationPage(IUserLocalService usersServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingPeriodSettingsViewModel();
                BindingContext = vm;

                PeriodLengthEntry.Value = LocalStorageService.UserCycleProfile.PeriodLengthDays.Value;
                CycleLengthEntry.Value = LocalStorageService.UserCycleProfile.CycleLengthDays.Value;  
                LMPDate.SelectedDate = LocalStorageService.UserCycleProfile.LastPeriodDate.Value; 

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

            LMPedit.IsVisible = isOpening;
            CycleLengthEdit.IsVisible = isOpening;
            PeriodLengthEdit.IsVisible = isOpening;
        }

        private void CloseModalPage()
        {
            EditDetailsBtn.Text = "Edit Details";
            FormBorder.IsVisible = false;

            LMPedit.IsVisible = false;
            CycleLengthEdit.IsVisible = false;
            PeriodLengthEdit.IsVisible = false;
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            CloseModalPage();
        }

        private void UpdateSingleItem(object sender, string param)
        {
            EditDetailsBtn.Text = "Close Editor";
            FormBorder.IsVisible = true;

            LMPedit.IsVisible = param == "LMP";
            CycleLengthEdit.IsVisible = param == "CycleLength";
            PeriodLengthEdit.IsVisible = param == "PeriodLength";
        }

        private async void SaveChangesTapped(object sender, EventArgs e)
        {
            if (CycleLengthEntry.Value <= 0 || PeriodLengthEntry.Value <= 0)
            {
                var error = "Please enter valid";
                if (CycleLengthEntry.Value <= 0) error += " Cycle Length,";
                if (PeriodLengthEntry.Value <= 0) error += " Period Length,";

                error = error.Substring(0, error.Length - 1);

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Inputs", error));

                return;
            }

            var localCycle = new
            {
                LastPeriodDate = LocalStorageService.UserCycleProfile.LastPeriodDate,
                CycleLengthDays = LocalStorageService.UserCycleProfile.CycleLengthDays,
                PeriodLengthDays = LocalStorageService.UserCycleProfile.PeriodLengthDays
            };

            try
            {
                await AppLoader.ShowAsync("Saving your changes...");

                var lmpChanged = localCycle.LastPeriodDate != LMPDate.SelectedDate;
                var cycleLengthChanged = localCycle.CycleLengthDays != CycleLengthEntry.Value;
                var periodLengthChanged = localCycle.PeriodLengthDays != PeriodLengthEntry.Value;

                LocalStorageService.UserCycleProfile.LastPeriodDate = lmpChanged ? LMPDate.SelectedDate : localCycle.LastPeriodDate;
                LocalStorageService.UserCycleProfile.CycleLengthDays = cycleLengthChanged ? CycleLengthEntry.Value : localCycle.CycleLengthDays;
                LocalStorageService.UserCycleProfile.PeriodLengthDays = periodLengthChanged ? PeriodLengthEntry.Value : localCycle.PeriodLengthDays;

                var updated = true;
                
                if (lmpChanged || cycleLengthChanged || periodLengthChanged)
                    updated = await _usersServ.UpdateUserCycleProfile();

                await AppLoader.HideAsync();

                if (updated)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your changes have been saved", "Ok"));
                    vm.UpdatePeriodDetails(LocalStorageService.UserCycleProfile);
                    CloseModalPage();
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Your changes were not saved, please try again.", "Ok"));

                    LocalStorageService.UserCycleProfile.LastPeriodDate = localCycle.LastPeriodDate;
                    LocalStorageService.UserCycleProfile.CycleLengthDays = localCycle.CycleLengthDays;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = localCycle.PeriodLengthDays;
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "An error occurred while saving your changes. Please try again.", "Ok"));
                await AppLoader.HideAsync();

                LocalStorageService.UserCycleProfile.LastPeriodDate = localCycle.LastPeriodDate;
                LocalStorageService.UserCycleProfile.CycleLengthDays = localCycle.CycleLengthDays;
                LocalStorageService.UserCycleProfile.PeriodLengthDays = localCycle.PeriodLengthDays;
            }
        }
    }
}
