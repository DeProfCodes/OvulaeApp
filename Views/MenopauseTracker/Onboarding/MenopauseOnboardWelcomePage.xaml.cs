using OvulaeApp.Views.GoalSetting;

namespace OvulaeApp.Views.MenopauseTracker.Onboarding
{
    public partial class MenopauseOnboardWelcomePage : ContentPage
    {
        public MenopauseOnboardWelcomePage()
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

        private async void StartMenopauseOnboardBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(MenopauseOnboardRenderPage));

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
