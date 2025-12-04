using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.ViewModels.Shared;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    public partial class MenopauseDashboardCalendarPage : ContentPage
    {
        private readonly IMenopauseService _menopauseServ;
        
        private MenopauseCalendarPageViewModel viewModel;

        public bool _isNavigating { get; set; }

        public MenopauseDashboardCalendarPage(IMenopauseService menopauseServ)
        {
            InitializeComponent();
            _menopauseServ = menopauseServ;
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                SetCrossComponents();

                viewModel = new MenopauseCalendarPageViewModel(_menopauseServ);
                viewModel.DayDetailsTray = DayDetailsTray;

                BindingContext = viewModel;

                //viewModel.Load();

                DayDetailsTray.BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnAppearing: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Failed to load page.", "OK");
            }
        }

        private void SetCrossComponents()
        {
            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });
        }

        private async void PreviousMonth_Tapped(object sender, TappedEventArgs e)
        {
            await NavigateWithSpinnerAsync(() => viewModel.GoPreviousMonth());
        }

        private async void NextMonth_Tapped(object sender, TappedEventArgs e)
        {
            await NavigateWithSpinnerAsync(() => viewModel.GoNextMonth());
        }

        private async Task NavigateWithSpinnerAsync(Action navigationAction)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                await Spinner.ShowSpinnerAsync();
                navigationAction.Invoke();
                await Task.Delay(100); // smooth animation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Navigation failed. Please try again.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }
    }
}
