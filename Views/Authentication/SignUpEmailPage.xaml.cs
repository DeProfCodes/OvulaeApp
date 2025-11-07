using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Services.APIs.Authentication;

namespace OvulaeApp.Views.Authentication
{
    public partial class SignUpEmailPage : ContentPage, ISpinnerService
    {
        private readonly IAuthenticationApi _authApi;
        
        public SignUpEmailPage(IAuthenticationApi authApi)
        {
            InitializeComponent();

            _authApi = authApi;
            LocalStorageService.Authenticated = false;

            if (Resources["NavigationsVM"] is NavigationsViewModel vm)
            {
                vm.SpinnerService = this;
            }
        }

        public async Task ShowSpinnerAsync()
        {
            await Spinner.ShowSpinnerAsync();
        }

        public async Task HideSpinner()
        {
            await Spinner.HideSpinnerAsync();
        }

        private void OnFieldsChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                var isValidEmail = ValidationsHelper.IsValidEmail(EmailEntry.Text);

                EmailValidation.IsVisible = !isValidEmail;
                AuthenticationPagesHelper.ToggleActionButtonButton(SignUpBtn1, isValidEmail);
            }
        }

        private async void SignUpBtn1_Clicked(object sender, EventArgs e)
        {
            KeyboardHelper.Dismiss();
            try
            {
                await AppLoader.ShowAsync("Checking email...");

                var email = EmailEntry.Text;
                var emailIsAvailable = await _authApi.EmailNotRegistered(email);

                if (emailIsAvailable)
                {
                    LocalStorageService.UserDetails.Email = email;

                    await AppLoader.HideAsync();

                    await Spinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(SignUpInfoPage));
                    await Spinner.HideSpinnerAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Email Exist", "This email is registered in our system, Please use different email."));
                }
            }
            catch
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Sign Up Error", "Failed to start signup. Unknown error."));
            }
            finally
            { 
                await AppLoader.HideAsync(); 
            }
        }

        private async void GoogleSignUpBtn_Tapped(object sender, TappedEventArgs e)
        {
            //await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Coming Soon!", "This will be implemented soon."));
        }

        private async void AppleSignUpBtn_Tapped(object sender, TappedEventArgs e)
        {
            //await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Coming Soon!", "This will be implemented soon."));
        }
    }

}
