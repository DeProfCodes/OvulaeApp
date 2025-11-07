using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Constants;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Services;


using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Auth;
using OvulaeShared.Services.APIs.Messaging;

namespace OvulaeApp.Views.Authentication
{
    public partial class PasswordResetEmailPage : ContentPage, ISpinnerService
    {
        private readonly IMessagingApi _messageApi;

        public PasswordResetEmailPage(IMessagingApi messageApi)
        {
            InitializeComponent();

            _messageApi = messageApi;

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

        private async void UseMobileInsteadLabelTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(PasswordResetMobilePage));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating to mobile password reset.", "OK");
            }
        }

        private void OnFieldsChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                var isValidEmail = ValidationsHelper.IsValidEmail(EmailEntry.Text);

                EmailValidation.IsVisible = !isValidEmail;

                AuthenticationPagesHelper.ToggleActionButtonButton(EmailCodeBtn, isValidEmail);
            }
        }

        private async void SendEmailCode_Clicked(object sender, EventArgs e)
        {
            this.DismissKeyboard();
            try
            {
                await AppLoader.ShowAsync("Sending Password Reset Email...");

                var emailResetCode = await _messageApi.SendPasswordResetEmail(EmailEntry.Text);
                if (emailResetCode.Success)
                {
                    AuthenticationPagesHelper.SetPasswordResetObject(emailResetCode.Message, PasswordResetType.Email, email: EmailEntry.Text);

                    await AppLoader.HideAsync();
                    await Task.Delay(CommonConstants.LOAD_DELAY);

                    var popup = new BrandedAlertPopup(
                        "Password Reset Email Sent",
                        "Check your mailbox for password reset OTP, if you don't find it in your inbox then check Spam folder.",
                        "Ok"
                    );

                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                    await popup.ShowWithResultAsync();

                    await Spinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(PasswordResetCodePage));
                    await Spinner.HideSpinnerAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Email Sending Failed", emailResetCode.Message));
                }
            }
            catch
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Email Sending Failed", "Failed to send Email password reset code. Unknown error."));
            }
            finally
            { 
                await AppLoader.HideAsync(); 
            }    
        }
    }

}
