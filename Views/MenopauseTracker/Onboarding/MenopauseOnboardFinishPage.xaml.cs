using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.Authentication;
using OvulaeShared.Enums;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;

namespace OvulaeApp.Views.MenopauseTracker.Onboarding
{
    public partial class MenopauseOnboardFinishPage : ContentPage
    {
        public MenopauseOnboardFinishPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var startTime = DateTime.Now;

                //await RunInitialPregnancySetupAsync();

                var timeElapsed = DateTime.Now - startTime;
                var minDuration = TimeSpan.FromSeconds(5);

                if (timeElapsed < minDuration)
                    await Task.Delay(minDuration - timeElapsed);

                await Shell.Current.GoToAsync(nameof(SignUpEmailPage));
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to load pregnancy finish page: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while finilizing information.", "OK");
            }
        }
    }
}
