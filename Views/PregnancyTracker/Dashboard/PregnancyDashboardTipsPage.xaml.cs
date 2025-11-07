using System.Reflection.PortableExecutable;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    [QueryProperty(nameof(Week), "week")]
    public partial class PregnancyDashboardTipsPage : ContentPage
    {
        private readonly ITipsService _tipsServ;
        private PregnancyTipsViewModel viewModel;

        public int Week { get; set; }

        public PregnancyDashboardTipsPage(ITipsService tipsServ)
        {
            InitializeComponent();
            _tipsServ = tipsServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (viewModel == null && Week > 0)
                {
                    viewModel = new PregnancyTipsViewModel(_tipsServ, Week);
                    BindingContext = viewModel;
                    UpdateProgressBarTipsSelected();

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load tips page, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening tips page.", "OK");
            }
        }

        private void TipCheckboxCard_Changed(TipCheckboxCard sender, bool isChecked)
        {
            UpdateProgressBarTipsSelected();
        }

        private void UpdateProgressBarTipsSelected()
        {
            try
            {
                if (BindingContext is PregnancyTipsViewModel vm)
                {
                    int tipsCompleted = vm.TipsData.Count(t => t.IsChecked);
                    TipsCompleteCount.Text = $"{tipsCompleted}/{vm.TipsData.Count} completed today!";
                    TipsCompleteBar.Progress = (tipsCompleted / (double)vm.TipsData.Count) * 100;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update progress bar: {ex.Message}");
            }
        }

        private async void SaveTips_Clicked(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Saving you favourite tips✨");
                var likedTipsIds = new List<int>();
                if (BindingContext is PregnancyTipsViewModel vm)
                {
                    likedTipsIds = vm.TipsData.Where(t => t.IsChecked).Select(t => t.TipItem.TipId).ToList();

                    if (likedTipsIds != null)
                    {
                        var saveRes = await _tipsServ.SaveLikedTips(ModuleType.Pregnancy, Week, likedTipsIds);

                        if (saveRes)
                        {
                            await AppLoader.HideAsync();
                            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved!", "Your favourite tips were saved 😊."));
                        }
                        else
                        {
                            await AppLoader.HideAsync();
                            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Did not save", "We couldn't save your tips🙁. Please try again later."));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to save your tips, error: {ex.Message}");
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Something went wrong, We couldn't save your tips🙁. Please try again later."));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }
    }
}
