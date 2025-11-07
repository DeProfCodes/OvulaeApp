
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.PrivacyPolicy;

namespace OvulaeApp.Views.Splashscreen
{
    public partial class SplashPrivacyPage : ContentPage
    {
        double xStart = 0;

        public SplashPrivacyPage()
        {
            InitializeComponent();

            LocalStorageService.Authenticated = false;
        }

        private async void AcceptAndContinueClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SplashscreenRenderPage));
        }

        private void OnAcceptAllTermsTapped(object sender, EventArgs e)
        {
            PrivacyPolicyTerms.CheckUncheck(true);
            ConsentDataSharingTerms.CheckUncheck(true);
            ConsentDataSharing2Terms.CheckUncheck(true);
            ConsentDataSharing3Terms.CheckUncheck(true);

            AcceptBtn.IsButtonEnabled = true;
        }

        private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Completed:
                    if (e.TotalX < -60)
                    {
                        // Swipe Left
                        await Navigation.PushAsync(new SplashscreenRenderPage());
                    }
                    else if (e.TotalX > 60)
                    {
                        // Swipe Right
                        await Navigation.PushAsync(new SplashWelcomePage());
                    }
                    break;
            }
        }

        private void ChecksTerms_CheckedChanged(object sender, bool e)
        {
            var allChecked = PrivacyPolicyTerms.IsChecked && ConsentDataSharingTerms.IsChecked;

            AcceptBtn.IsButtonEnabled = allChecked;
        }

        private async void PrivacyLink_Tapped(object sender, TappedEventArgs e)
        {
            await Spinner.ShowSpinnerAsync();

            await Shell.Current.GoToAsync(nameof(PrivacyPolicyPage));

            await Spinner.HideSpinnerAsync();
        }

        private async void TermsOfUseLink_Tapped(object sender, TappedEventArgs e)
        {
            await Spinner.ShowSpinnerAsync();

            await Shell.Current.GoToAsync(nameof(TermsOfUsePage));

            await Spinner.HideSpinnerAsync();
        }

        private void PolicyLineTapped(object sender, TappedEventArgs e)
        {
            PrivacyPolicyTerms.IsChecked = !PrivacyPolicyTerms.IsChecked;
        }

        private void TermsCheckTapped(object sender, TappedEventArgs e)
        {
            ConsentDataSharingTerms.IsChecked = !ConsentDataSharingTerms.IsChecked;
        }

        private void TermsCheckTapped2(object sender, TappedEventArgs e)
        {
            ConsentDataSharing2Terms.IsChecked = !ConsentDataSharing2Terms.IsChecked;
        }

        private void TermsCheckTapped3(object sender, TappedEventArgs e)
        {
            ConsentDataSharing3Terms.IsChecked = !ConsentDataSharing3Terms.IsChecked;
        }
    }

}
