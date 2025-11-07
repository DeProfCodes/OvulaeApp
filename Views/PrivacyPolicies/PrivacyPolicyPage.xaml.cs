using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.Views.PrivacyPolicy
{
    public partial class PrivacyPolicyPage : ContentPage
    {
        public PrivacyPolicyPage()
        {
            InitializeComponent();

            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.SetLoaders(Spinner, AppLoader);
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });

            Header.IsVisible = LocalStorageService.Authenticated;
            BaseTabs.IsVisible = LocalStorageService.Authenticated;
        }

        private async void OnPrivacyLinkTapped(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://ovulae.com/privacy");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open link.", "OK");
            }
        }

        private async void OnWebsiteTapped(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://ovulae.com");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open link.", "OK");
            }
        }

        private async void OnPrivacyEmailTapped(object sender, TappedEventArgs e)
        {
            try
            {
                string email = "privacy@ovulae.com";
                string subject = Uri.EscapeDataString("Privacy Inquiry");
                string body = Uri.EscapeDataString("Hi Ovulae Team,\n\nI have a question regarding your privacy policy...");

                var mailtoUri = new Uri($"mailto:{email}?subject={subject}&body={body}");
                await Launcher.Default.OpenAsync(mailtoUri);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open mail app.", "OK");
            }
        }
    }

}
