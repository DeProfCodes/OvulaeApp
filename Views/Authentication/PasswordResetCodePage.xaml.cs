using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using OvulaeApp.Helpers.Constants;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Pages.Authentication;



using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Auth;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Authentication;
using OvulaeShared.Services.APIs.Messaging;

namespace OvulaeApp.Views.Authentication
{
    public partial class PasswordResetCodePage : ContentPage, ISpinnerService
    {
        private readonly IAuthenticationApi _authApi;
        private readonly IMessagingApi _messageApi;

        double trayStartY;
        bool isDragging = false;

        public PasswordResetCodePage(IAuthenticationApi authApi, IMessagingApi messageApi)
        {
            InitializeComponent();

            _authApi = authApi;
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

        private async Task HideBottomTrayAsync()
        {
            await BottomTray.TranslateTo(0, 300, 300, Easing.SinIn);
            BottomTray.IsVisible = false;
            OverlayBackground.IsVisible = false;
        }

        private async Task ShowBottomTrayAsync()
        {
            OverlayBackground.IsVisible = true;
            BottomTray.IsVisible = true;
            await BottomTray.TranslateTo(0, 0, 300, Easing.SinOut);
        }

        private async void Otp1_Focused(object sender, FocusEventArgs e)
        {
            var clipboardText = await Clipboard.GetTextAsync();
            /*
            if (!string.IsNullOrEmpty(clipboardText) && clipboardText.Length == 6 && clipboardText.All(char.IsDigit))
            {
                Otp1.Text = clipboardText[0].ToString();
                Otp2.Text = clipboardText[1].ToString();
                Otp3.Text = clipboardText[2].ToString();
                Otp4.Text = clipboardText[3].ToString();
                Otp5.Text = clipboardText[4].ToString();
                Otp6.Text = clipboardText[5].ToString();

                Otp6.Focus();
            }
            */
        }

        private void OtpEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && e.NewTextValue?.Length == 1)
            {
                string text = e.NewTextValue;
                if (string.IsNullOrEmpty(text))
                    return;

                // Check if user deleted text
                if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
                {
                    // Go back to previous Entry
                    if (entry == Otp6) Otp5.Focus();
                    else if (entry == Otp5) Otp4.Focus();
                    else if (entry == Otp4) Otp3.Focus();
                    else if (entry == Otp3) Otp2.Focus();
                    else if (entry == Otp2) Otp1.Focus();
                }

                // Handle pasting 6-digit code into first box
                if (entry == Otp1 && text.Length == 6)
                {
                    Otp1.Text = text[0].ToString();
                    Otp2.Text = text[1].ToString();
                    Otp3.Text = text[2].ToString();
                    Otp4.Text = text[3].ToString();
                    Otp5.Text = text[4].ToString();
                    Otp6.Text = text[5].ToString();
                    Otp6.Focus();
                    return;
                }

                // Move to next box if 1 char entered
                if (text.Length == 1)
                {
                    if (entry == Otp1) Otp2.Focus();
                    else if (entry == Otp2) Otp3.Focus();
                    else if (entry == Otp3) Otp4.Focus();
                    else if (entry == Otp4) Otp5.Focus();
                    else if (entry == Otp5) Otp6.Focus();
                    else if (entry == Otp6)
                    {
                        entry.Unfocus();
                    }
                }

                var allOtpFilled = !string.IsNullOrEmpty(Otp1.Text) && !string.IsNullOrEmpty(Otp2.Text) && !string.IsNullOrEmpty(Otp3.Text) &&
                                   !string.IsNullOrEmpty(Otp4.Text) && !string.IsNullOrEmpty(Otp5.Text) && !string.IsNullOrEmpty(Otp6.Text);

                AuthenticationPagesHelper.ToggleActionButtonButton(VerifySMSCodeBtn, allOtpFilled);
            }
        }

        private async void VerifySMSCodeBtnClicked(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Verifying Code...");

                var otpCode = $"{Otp1.Text}{Otp2.Text}{Otp3.Text}{Otp4.Text}{Otp5.Text}{Otp6.Text}";

                await Task.Delay(3000);

                if (otpCode == LocalStorageService.PasswordReset.OTP)
                {
                    await AppLoader.HideAsync();
                    await ShowBottomTrayAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Incorrect Code", "Code entered is incorrect. If the code no longer works, request a new one."));
                }
            }
            catch (Exception ex)
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Incorrect Code", "Failed to validate code. Unknown error."));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            var password = e.NewTextValue ?? string.Empty;

            AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseLength, LabelLength, password.Length >= 8);
            AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseNumber, LabelNumber, password.Any(char.IsDigit));
            AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseUpper, LabelUpper, password.Any(char.IsUpper));
            AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseLower, LabelLower, password.Any(char.IsLower));
            AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseSpecial, LabelSpecial, password.Any(ch => !char.IsLetterOrDigit(ch)));

            var passwordMatch = NewPasswordEntry.Text == ConfirmPasswordEntry.Text;

            PasswordValidation.IsVisible = !passwordMatch;

            AuthenticationPagesHelper.ToggleActionButtonButton(PasswordResetBtn, passwordMatch);
        }

        private void OnPasswordConfirmTextChanged(object sender, TextChangedEventArgs e)
        {
            var password = e.NewTextValue ?? string.Empty;

            var passwordMatch = NewPasswordEntry.Text == ConfirmPasswordEntry.Text;

            PasswordValidation.IsVisible = !passwordMatch;

            AuthenticationPagesHelper.ToggleActionButtonButton(PasswordResetBtn, passwordMatch);
        }

        private async void OnSavePasswordClicked(object sender, EventArgs e)
        {
            this.DismissKeyboard();
            try
            {
                await AppLoader.ShowAsync("Reseting Password...");

                var passChangeResult = new GenericResult();
                var localInfo = LocalStorageService.PasswordReset;
                string newPassword = NewPasswordEntry.Text;

                if (localInfo.Type == PasswordResetType.Mobile)
                {
                    passChangeResult = await _authApi.ChangePasswordMobile(localInfo.CountryCode, localInfo.PhoneNumber, newPassword);
                }
                else if (localInfo.Type == PasswordResetType.Email)
                {
                    passChangeResult = await _authApi.ChangePasswordEmail(localInfo.Email, newPassword);    
                }

                if (passChangeResult.Success)
                {
                    await AppLoader.HideAsync();
                    await HideBottomTrayAsync();

                    var popup = new BrandedAlertPopup(
                        "Password Reset Success",
                        "Your password has been reset, you are redirected to Login page."
                    );

                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                    await popup.ShowWithResultAsync();

                    await Spinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                    await Spinner.HideSpinnerAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Reset Fail", "Could not change your password."));
                }
            }
            catch
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Reset Fail", "Failed to change your password. Unknown error."));
            }
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await HideBottomTrayAsync();
        }

        private async void ResendCodeToChosenMethod(object sender, TappedEventArgs e)
        {
            try
            {
                var passwordResendResult = new GenericResult();
                var passResetInfo = LocalStorageService.PasswordReset;

                if (passResetInfo.Type == PasswordResetType.Email)
                {
                    await AppLoader.ShowAsync($"Sending Password Reset to {passResetInfo.Email}...");
                    passwordResendResult = await _messageApi.SendPasswordResetEmail(passResetInfo.Email);
                }
                else if (LocalStorageService.PasswordReset.Type == PasswordResetType.Mobile)
                {
                    await AppLoader.ShowAsync($"Sending Password Reset SMS to {passResetInfo.CountryCode}{passResetInfo.PhoneNumber}...");
                    passwordResendResult = await _messageApi.SendPasswordResetSMS(passResetInfo.CountryCode, passResetInfo.PhoneNumber);
                }

                if (passwordResendResult.Success)
                {
                    LocalStorageService.PasswordReset.OTP = passwordResendResult.Message;

                    await AppLoader.HideAsync();

                    var popup = new BrandedAlertPopup(
                        "Password Reset Sent",
                        "Your password reset code has been sent, check your inbox."
                    );

                    await Shell.Current.CurrentPage.ShowPopupAsync(popup);
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Resend Failed", "Failed to resend OTP, try again."));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Resend Failed", "Failed to resend OTP, try again."));
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (NewPasswordEntry.IsPassword)
            {
                PasswordCover.Source = "eye_hide";
                PasswordCover2.Source = "eye_hide";
                NewPasswordEntry.IsPassword = false;
                ConfirmPasswordEntry.IsPassword = false;
            }
            else
            {
                PasswordCover.Source = "eye_show";
                PasswordCover2.Source = "eye_show";
                NewPasswordEntry.IsPassword = true;
                ConfirmPasswordEntry.IsPassword = true;
            }
        }
    }

}
