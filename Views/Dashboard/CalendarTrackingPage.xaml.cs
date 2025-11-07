using System.Reflection.PortableExecutable;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.ViewModels.Calendar;
using OvulaeApp.Views.Components.Dashboard;

namespace OvulaeApp.Views.Dashboard
{
    public partial class CalendarTrackingPage : ContentPage
    {
        private CalendarPageViewModel viewModel;

        public CalendarTrackingPage()
        {
            InitializeComponent();
            BaseTabs.SetSpinnerLoader(Spinner);

            viewModel = new CalendarPageViewModel();

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            SideMenu.SetSpinnerLoader(Spinner);
            Header.SetSpinnerLoader(Spinner);
            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            base.OnAppearing();
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
            try
            {
                await Spinner.ShowSpinnerAsync();
                navigationAction.Invoke();
                // If LoadMonth is async or heavy, consider making VM method async and await here
                await Task.Delay(300); // simulate delay if needed
            }
            catch (Exception ex)
            {
                // Optional: log the error or show a friendly message
                Console.WriteLine($"Navigation failed: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }
    }
}
