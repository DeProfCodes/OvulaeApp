using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.Views.PrivacyPolicy
{
    public partial class TermsOfUsePage : ContentPage
    {
        public TermsOfUsePage()
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

        private async void OnTermsLinkTapped(object sender, TappedEventArgs e)
        {
            try
            {
                Uri uri = new("https://ovulae.com/terms");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Unable to open the link.", "OK");
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

        private async void OnSupportEmailTapped(object sender, TappedEventArgs e)
        {
            try
            {
                string email = "support@ovulae.com";
                string subject = Uri.EscapeDataString("Terms Inquiry");
                string body = Uri.EscapeDataString("Hi Ovulae Team,\n\nI have a question regarding your terms of use...");

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
