using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using System;

namespace OvulaeApp.Views.About
{
    public partial class AboutUsPage : ContentPage
    {
        public AboutUsPage()
        {
            InitializeComponent();

            BaseTabs.SetLoaders(Spinner, AppLoader);
            SideMenu.ConfigureComponents(
                Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoModal, MenopauseTrackerOnBoard,
                PregnancyComplete
            );
            Header.SetLoaders(Spinner, AppLoader);

            Header.OpenSideMenuCommand = new Command(async () =>
            {
                await SideMenu.OpenAsync();
            });
        }

        private async void OnEmailTapped(object sender, EventArgs e)
        {
            try
            {
                await Launcher.Default.OpenAsync("mailto:support@ovulae.com");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open email client.", "OK");
            }
        }

        private async void OnWebsiteTapped(object sender, EventArgs e)
        {
            try
            {
                await Launcher.Default.OpenAsync("https://ovulae.com");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open the website.", "OK");
            }
        }
    }
}
