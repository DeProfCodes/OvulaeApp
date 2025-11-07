using System.Net.Mail;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using OvulaeApp.Helpers.Constants;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Models.Shared;
using OvulaeApp.Services;

using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.GoalSetting;

namespace OvulaeApp.Views.PregnancyTracker.Onboarding
{
    public partial class PregnancyOnboardWelcomePage : ContentPage
    {
        public PregnancyOnboardWelcomePage()
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

        private async void StartPregOnboardBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(PregnancyOnboardRenderPage));

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
