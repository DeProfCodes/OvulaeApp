using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Constants;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Services;


using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Auth;
using OvulaeShared.Services.APIs.Messaging;

namespace OvulaeApp.Views.Authentication
{
    public partial class PasswordResetMobilePage : ContentPage, ISpinnerService
    {
        private readonly IMessagingApi _messageApi;

        public PasswordResetMobilePage(IMessagingApi messageApi)
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

        private async void UseEmailInsteadLabelTapped(object sender, TappedEventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(PasswordResetEmailPage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating to email password reset.", "OK");
            }
        }

        private async void SendSMSCode_Clicked(object sender, EventArgs e)
        {
            this.DismissKeyboard();
            try
            {
                await AppLoader.ShowAsync("Sending Password Reset SMS...");

                var countryCode = CountryCode.SelectedItem.ToString();
                countryCode = $"+{countryCode.Split("+")[1]}";

                var phoneNumber = PhoneNumberEntry.Text;

                var smsResetCode = await _messageApi.SendPasswordResetSMS(countryCode, phoneNumber);
                if (smsResetCode.Success)
                {
                    AuthenticationPagesHelper.SetPasswordResetObject(smsResetCode.Message, PasswordResetType.Mobile, countryCode: countryCode, phoneNumber: phoneNumber);

                    await AppLoader.HideAsync();
                    await Task.Delay(CommonConstants.LOAD_DELAY);

                    var popup = new BrandedAlertPopup(
                        "Password Reset SMS Sent",
                        "Check your SMS inbox for OTP code.",
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
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("SMS Sending Failed", "Your number is not registered with Ovulae, please Sign up."));
                }
            }
            catch
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("SMS Sending Failed", "Failed to send SMS code. Unknown error."));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private void PhoneNumberEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                string raw = entry.Text ?? string.Empty;

                string digits = new string(raw.Where(char.IsDigit).ToArray());

                if (digits.StartsWith("0"))
                    digits = digits.TrimStart('0');

                if (digits.Length > 15)
                    digits = digits.Substring(0, 15);

                if (entry.Text != digits)
                    entry.Text = digits;

                bool isValid = digits.Length >= 8 && digits.Length <= 15;

                PhoneValidationLabel.IsVisible = !isValid;

                AuthenticationPagesHelper.ToggleActionButtonButton(SendCodeButton, isValid);
            }
        }
    }
}
