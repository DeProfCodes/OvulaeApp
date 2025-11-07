namespace OvulaeApp.Views.Splashscreen
{
    public partial class SplashWelcomePage : ContentPage
    {
        public SplashWelcomePage()
        {
            InitializeComponent();
        }

        private async void GetStartedClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SplashPrivacyPage));
        }
    }

}
