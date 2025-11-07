using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.Views.Settings.EditSettingsPages.MenopauseSettings;

public partial class EditMenopauseProfilePage : ContentPage
{
    private EditSettingMenopauseViewModel vm;
    private readonly IUserLocalService _userServ;

    public EditMenopauseProfilePage(IUserLocalService userServ)
    {
        InitializeComponent();
        _userServ = userServ;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm = new EditSettingMenopauseViewModel();
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

        //LastPeriodPicker.SelectedDate = LocalStorageService.UserCycleProfile.LastPeriodDate ?? DateTime.Today;
        //HormonalSwitch.IsToggled = LocalStorageService.UserCycleProfile.Treatments.Contains(TreatmentTypes.HRT.GetDisplayDescription());
    }

    private async void BackButtonTapped(object sender, TappedEventArgs e)
    {
        VisualEventsHelper.TapDimEffectGray(BackBtnBorder);
        await Shell.Current.GoToAsync("..");
    }

    private void EditDetailsButtonTapped(object sender, EventArgs e)
    {
        var isEditing = !FormBorder.IsVisible;
        EditDetailsBtn.Text = isEditing ? "Close Editor" : "Edit Profile";
        FormBorder.IsVisible = isEditing;

        LMPedit.IsVisible = isEditing;
        HormanalTreatment.IsVisible = isEditing;
    }

    private async void SaveChangesTapped(object sender, EventArgs e)
    {
        var oldLMP = LocalStorageService.UserCycleProfile.LastPeriodDate;
        try
        {
            
            await AppLoader.ShowAsync("Saving changes...");

            LocalStorageService.UserCycleProfile.LastPeriodDate = LMPDate.SelectedDate;
            var usesHormones = HormonalTreatment.SelectedIndex == 0;

            if (usesHormones)
            {
                LocalStorageService.UserCycleProfile.Treatments.Add(TreatmentTypes.HRT.GetDisplayDescription());
            }
            else
            {
                LocalStorageService.UserCycleProfile.Treatments.Remove(TreatmentTypes.HRT.GetDisplayDescription());
            }

            var updated = await _userServ.UpdateUserCycleProfile();

            await AppLoader.HideAsync();

            if (updated)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your changes have been saved.", "Ok"));
                vm.UpdateFromProfile(true);
                FormBorder.IsVisible = false;
                EditDetailsBtn.Text = "Edit Profile";
            }
            else
            {
                LocalStorageService.UserCycleProfile.LastPeriodDate = oldLMP;
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Could not save changes. Please try again.", "Ok"));
            }
        }
        catch
        {
            LocalStorageService.UserCycleProfile.LastPeriodDate = oldLMP;
            await AppLoader.HideAsync();
            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong while saving.", "Ok"));
        }
    }

    private void CloseModalPage()
    {
        EditDetailsBtn.Text = "Edit Profile";
        FormBorder.IsVisible = false;

        LMPedit.IsVisible = false;
        HormanalTreatment.IsVisible = false;
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
        HormanalTreatment.IsVisible = param == "HRT";
    }
}
