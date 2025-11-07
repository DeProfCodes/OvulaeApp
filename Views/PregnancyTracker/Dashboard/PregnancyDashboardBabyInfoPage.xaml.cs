using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.ViewModels.PregnancyTracker;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    [QueryProperty(nameof(Week), "week")]
    public partial class PregnancyDashboardBabyInfoPage : ContentPage
    {
        private IPregnancyService _pregServ;
        private PregnancyBabyDevelopmentViewModel viewModel;

        public int Week { get; set; }

        public bool _isNavigating { get; set; }

        public PregnancyDashboardBabyInfoPage(IPregnancyService pregServ)
        {
            InitializeComponent();

            _pregServ = pregServ;
            
            BaseTabs.SetLoaders(Spinner, AppLoader);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (viewModel == null && Week > 0)
                {
                    viewModel = new PregnancyBabyDevelopmentViewModel(_pregServ, Week);
                    BindingContext = viewModel;
                    UpdateWeekTexts();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Baby development page opening failed: {ex.Message}");
            }
        }

        private void UpdateWeekTexts()
        {
            try
            {
                PrevWeekText.Text = $"Week {viewModel.Week - 1}";
                CurrentWeek.Text = $"Week {viewModel.Week}";
                NextWeekText.Text = $"Week {viewModel.Week + 1}";
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Baby development week change failed: {ex.Message}");
            }
        }

        private void NavigateWeeks_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating || viewModel.IsBusy)
                return;

            viewModel.IsBusy = true;
            _isNavigating = true;

            try
            {
                if (sender is Border)
                {
                    var btn = sender as Border;
                    if (btn == PrevWeekBtnBrd)
                    {
                        viewModel.GoToPreviousWeek();
                        PrevWeekBtn.IsVisible = viewModel.Week > 0;
                    }
                    else if (btn == NextWeekBtnBrd)
                    {
                        viewModel.GoToNextWeek();
                        NextWeekBtn.IsVisible = viewModel.Week < 45;
                    }
                    UpdateWeekTexts();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Baby development week change failed: {ex.Message}");
            }
            finally
            {
                viewModel.IsBusy = false;
                _isNavigating = false;
            }
        }
    }
}
