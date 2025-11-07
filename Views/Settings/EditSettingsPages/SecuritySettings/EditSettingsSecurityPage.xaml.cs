using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.PrivacyPolicy;
using OvulaeShared.Enums.Auth;
using OvulaeShared.Enums.Errors;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Authentication;
using OvulaeShared.Services.APIs.Messaging;
using OvulaeShared.Services.APIs.Users;

namespace OvulaeApp.Views.Settings.EditSettingsPages.SecuritySettings
{
    public partial class EditSettingsSecurityPage : ContentPage
    {
        private readonly IMessagingApi _messageApi;
        private readonly IUserLocalService _usersServ;
        private readonly IAuthenticationApi _authApi;
        private readonly IUsersApi _userApi;

        private string resetType;
        private string RawOTPCode;

        public EditSettingsSecurityPage(IUserLocalService usersServ, IMessagingApi messagingApi, IAuthenticationApi authApi, IUsersApi userApi)
        {
            InitializeComponent();

            _usersServ = usersServ;
            _messageApi = messagingApi;
            _authApi = authApi;
            _userApi = userApi;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });
            }
            catch (Exception ex)
            {

            }
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            VisualEventsHelper.TapDimEffectGray(BackBtnBorder);
            await Shell.Current.GoToAsync("..");
        }

        private void CloseEditorModal()
        {
            FormBorder.IsVisible = false;

            PasswordChangeEdit.IsVisible = false;
            MobileChangeEdit.IsVisible = false;
            NewPasswordReset.IsVisible = false;
            PhoneNumberChange.IsVisible = false;
            //ConfirmOTPBtn.IsVisible = false;
            UpdateSecurityBtn.IsVisible = false;
            ResetOTP.IsVisible = false;
            OTPResetGroup.IsVisible = false;
            DeleteMyAccount.IsVisible = false;
        }

        private void CloseModalPage(object sender, EventArgs e)
        {
            CloseEditorModal();
        }

        public void RequestSecurityChange(object sender, string param)
        {
            resetType = param;

            FormBorder.IsVisible = true;
            PasswordChangeEdit.IsVisible = resetType == "Password";
            MobileChangeEdit.IsVisible = resetType == "Mobile";
            DeleteMyAccount.IsVisible = resetType == "DeleteAccount";
        }

        private void MessageOTPType_SelectionChanged(object sender, EventArgs e)
        {
            MessageMethodHint.Text = MessageOTPType.SelectedValue switch
            {
                "Email" => $"An email will be sent to {LocalStorageService.UserDetails.Email}",
                "SMS" => $"An SMS will be sent to {LocalStorageService.UserDetails.CountryCode}{LocalStorageService.UserDetails.PhoneNumber}",
                _ => "Please select a valid method."
            };
        }

        private async Task SendOTP()
        {
            try
            {
                await AppLoader.ShowAsync("Sending OTP...");

                GenericResult otpSendResult = new();
                if (resetType == "Password" && MessageOTPType.SelectedValue == "SMS")
                {
                    otpSendResult = await _messageApi.SendPasswordResetSMS(LocalStorageService.UserDetails.CountryCode, LocalStorageService.UserDetails.PhoneNumber);
                }
                else if (resetType == "Mobile" || MessageOTPType.SelectedValue == "Email")
                {
                    otpSendResult = await _messageApi.SendPasswordResetEmail(LocalStorageService.UserDetails.Email);
                }

                await AppLoader.HideAsync();

                if (otpSendResult.Success)
                {
                    RawOTPCode = otpSendResult.Message;

                    ResetOTP.IsVisible = true;
                    OTPResetGroup.IsVisible = true;

                    PasswordChangeEdit.IsVisible = false;
                    MobileChangeEdit.IsVisible = false;
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Unable to send your One Time Pin at this moment, please try again later.", "Ok"));
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed", "Unable to send your One Time Pin at this moment, please try again later.", "Ok"));
            }
        }

        public async void SendSecurityReset(object sender, string param)
        {
            await SendOTP();
        }

        private async void ResendOTPButton_Tapped(object sender, EventArgs e)
        {
            await SendOTP();
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

                ConfirmOTPBtn.IsButtonEnabled = allOtpFilled;
            }
        }

        private async void ConfirmOTPBtn_Tapped(object sender, EventArgs e)
        {
            try
            {
                string enteredOtp = $"{Otp1.Text}{Otp2.Text}{Otp3.Text}{Otp4.Text}{Otp5.Text}{Otp6.Text}";
                if (RawOTPCode == enteredOtp)
                {
                    NewPasswordReset.IsVisible = resetType == "Password";
                    PhoneNumberChange.IsVisible = resetType == "Mobile";

                    OTPResetGroup.IsVisible = false;

                    UpdateSecurityBtn.IsVisible = true;

                    Otp1.Text = "";
                    Otp2.Text = "";
                    Otp3.Text = "";
                    Otp4.Text = "";
                    Otp5.Text = "";
                    Otp6.Text = "";
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "The One Time Pin you entered is incorrect. Please try again.", "Ok"));
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "An error occurred while confirming your OTP. Please try again.", "Ok"));
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

            UpdateSecurityBtn.IsButtonEnabled = passwordMatch;
        }

        private void OnPasswordConfirmTextChanged(object sender, TextChangedEventArgs e)
        {
            var password = e.NewTextValue ?? string.Empty;

            var passwordMatch = NewPasswordEntry.Text == ConfirmPasswordEntry.Text;

            PasswordValidation.IsVisible = !passwordMatch;

            UpdateSecurityBtn.IsButtonEnabled = passwordMatch;
        }

        private void TogglePasswordVisibilityTapped(object sender, TappedEventArgs e)
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

                UpdateSecurityBtn.IsButtonEnabled = isValid;
            }
        }

        private async void UpdateSecurityBtn_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (resetType == "Password")
                {
                    await AppLoader.ShowAsync("Reseting Password...");

                    var passChangeResult = new GenericResult();
                    string newPassword = NewPasswordEntry.Text;

                    passChangeResult = await _authApi.ChangePasswordEmail(LocalStorageService.UserDetails.Email, newPassword);
                    
                    if (passChangeResult.Success)
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Changed!", "Your password has been successfully changed. You can use your new password when you login again.", "Ok"));

                        CloseEditorModal();
                    }
                    else
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Password Reset Fail", "Could not change your password."));
                    }
                }
                else if (resetType == "Mobile")
                {
                    await AppLoader.ShowAsync("Changing Phone Number...");

                    var countryCode = $"+{CountryCode.SelectedItem.ToString().Split("+")[1]}";
                    var phoneNumber = PhoneNumberEntry.Text.Trim();

                    var phoneChangeResult = await _usersServ.UpdatePhoneNumber(countryCode, phoneNumber);

                    if (phoneChangeResult.Success)
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Phone Number Changed!", "Your phone number has been successfully changed.", "Ok"));

                        CloseEditorModal();
                    }
                    else if (phoneChangeResult.ErrorTypes.Contains(ErrorTypes.AUTH_PHONE_NUMBER_EXIST))
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Phone Number Already Exists", "The phone number you are trying to change to is already in use. Please use a different number.", "Ok"));
                    }
                    else
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Phone Number Change Fail", "Failed to change your phone number. Please try again later."));
                    }
                }
            }
            catch
            {
                var title = resetType == "Password" ? "Password Change Error" : "Phone Number Change Error";
                var message = resetType == "Password" ? "An error occurred while changing your password. Please try again." : "An error occurred while changing your phone number. Please try again.";
                
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup(title, message));
            }
        }

        private async void OpenPrivacyPolicy(object sender, EventArgs e)
        {
            try
            {
                await Spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync(nameof(PrivacyPolicyPage));

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void DeleteMyAccountTapped(object sender, EventArgs e)
        {
            try
            {
                var today = DateTime.Now.Date;
                var nextPaymentDate = LocalStorageService.UserSubscription.NextPaymentDate.Date;

                var subscriptionDaysLeft = (nextPaymentDate - today).Days;
                subscriptionDaysLeft = Math.Max(subscriptionDaysLeft, 0);

                var confirmSubscriptionTerminate = await YesNoModal.ShowYesNoModal("Cancel Subscription?", $"Your current subsctription is still left with {subscriptionDaysLeft} Days, are you sure you want to proceed with ACCOUNT DELETION?");
                if (confirmSubscriptionTerminate == ModalCloseType.Accept)
                {
                    var confirm = await YesNoModal.ShowYesNoModal("DELETE ACCOUNT?", "ARE YOU SURE YOU WANT TO DELETE YOUR ACCOUNT? ONCE DELETED, IT CANNOT BE REVERSED!");
                    if (confirm == ModalCloseType.Accept)
                    {
                        await AppLoader.ShowAsync("Deleting your account...");
                        var deleteAccountResult = await _userApi.DeleteUserFullAccount(LocalStorageService.UserDetails.UserId);
                        await AppLoader.HideAsync();

                        if (deleteAccountResult.Success)
                        {
                            var popup = new BrandedAlertPopup("ACCOUNT DELETED", "Your account has been deleted. You will be logged out now and will not be able to login with same details unless you create a new account.");

                            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

                            await popup.ShowWithResultAsync();

                            await _usersServ.Logout();

                            await AppLoader.ShowAsync("Logging out...");
                            await Shell.Current.GoToAsync(nameof(LoginPage));
                            await AppLoader.HideAsync();
                        }
                        else
                        {
                            await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Deletion Failed", "Something went wrong whilst attempting to delete your data. Please try again, if the error persist please contact support@ovulae.com and request for your data to be deleted."));
                        }
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}
