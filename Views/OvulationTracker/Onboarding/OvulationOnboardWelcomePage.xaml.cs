using OvulaeApp.Views.GoalSetting;

namespace OvulaeApp.Views.OvulationTracker.Onboarding
{
    public partial class OvulationOnboardWelcomePage : ContentPage
    {
        public OvulationOnboardWelcomePage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                Shell.Current.GoToAsync(nameof(GoalSettingPage));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
            }
            return base.OnBackButtonPressed();
        }

        private async void StartOvulationOnboardBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(OvulationOnboardRenderPage));

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating to onboarding questions.", "OK");
            }
        }
    }
}
