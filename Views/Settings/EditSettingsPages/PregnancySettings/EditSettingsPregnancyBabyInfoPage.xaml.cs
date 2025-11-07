using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;

namespace OvulaeApp.Views.Settings.EditSettingsPages.PregnancySettings
{
    public partial class EditSettingsPregnancyBabyInfoPage : ContentPage
    {
        private EditSettingsPregnancySettingsViewModel vm;

        private readonly IUserLocalService _usersServ;
        private ModalUpdateType ChangeType;

        public EditSettingsPregnancyBabyInfoPage(IUserLocalService usersServ)
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

        private void UpdateSingleItem(object sender, string param)
        {
            EditDetailsBtn.Text = "Close Editor";
            FormBorder.IsVisible = true;

            BabyNameEdit.IsVisible = param == "BabyName";
            BabyGenderEdit.IsVisible = param == "BabyGender";
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            EditDetailsBtn.Text = "Close Editor";
            FormBorder.IsVisible = false;

            BabyNameEdit.IsVisible = false;
            BabyGenderEdit.IsVisible = false;
        }

        private void EditDetailsButtonTapped(object sender, EventArgs e)
        {
            var isOpening = EditDetailsBtn.Text.ToLower().Contains("edit details");
            
            EditDetailsBtn.Text = isOpening ? "Close Editor" : "Edit Details";
            FormBorder.IsVisible = isOpening;
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
