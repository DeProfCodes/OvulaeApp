using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Payments;
using OvulaeApp.Services.UserInterface.Components;
using OvulaeApp.ViewModels.Shared;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Subscription;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Errors;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.User;
using OvulaeShared.Services.APIs.Authentication;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Views.Authentication
{
    public partial class SignUpInfoPage : ContentPage, ISpinnerService
    {
        private readonly IAuthenticationApi _authApi;
        private readonly IUsersApi _userApi;
        private readonly IUserLocalService _usersServ;
        private readonly ISubscriptionPaymentService _applePay;

        public SignUpInfoPage(IAuthenticationApi authApi, IUsersApi userApi, IUserLocalService usersServ, ISubscriptionPaymentService applePay)
        {
            InitializeComponent();

            _authApi = authApi;
            _userApi = userApi;
            _usersServ = usersServ;
            _applePay = applePay;

            if (Resources["NavigationsVM"] is NavigationsViewModel vm)
            {
                vm.SpinnerService = this;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(200);

            HiddenUnfocusEntry.Focus();
            await Task.Delay(100);
            HiddenUnfocusEntry.Unfocus();
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
                if (entry == Firstname)
                {
                    FirstnameValidation.IsVisible = Firstname.Text == "";
                }
                else if (entry == Lastname)
                {
                    LastnameValidation.IsVisible = Lastname.Text == "";
                }
                else if (entry == PhoneNumber)
                {
                    string raw = PhoneNumber.Text ?? string.Empty;

                    string digits = new string(raw.Where(char.IsDigit).ToArray());

                    if (digits.StartsWith("0"))
                        digits = digits.TrimStart('0');

                    if (digits.Length > 15)
                        digits = digits.Substring(0, 15);

                    if (PhoneNumber.Text != digits)
                        PhoneNumber.Text = digits;

                    bool isValid = digits.Length == 0 && digits.Length >= 8 && digits.Length <= 15;

                    PhoneNumberValidation.IsVisible = !isValid;
                }
                else if (entry == PasswordEntry)
                {
                    var password = e.NewTextValue ?? string.Empty;

                    AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseLength, LabelLength, password.Length >= 8);
                    AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseNumber, LabelNumber, password.Any(char.IsDigit));
                    AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseUpper, LabelUpper, password.Any(char.IsUpper));
                    AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseLower, LabelLower, password.Any(char.IsLower));
                    AuthenticationPagesHelper.UpdatePasswordStrenthUI(EllipseSpecial, LabelSpecial, password.Any(ch => !char.IsLetterOrDigit(ch)));   
                }
                else if (entry == ConfirmPasswordEntry)
                {
                    PasswordValidation.IsVisible = PasswordEntry.Text != ConfirmPasswordEntry.Text;
                }
            }

            if (Firstname.Text != null && Lastname.Text != null && PasswordEntry.Text != null && ConfirmPasswordEntry.Text != null)
            {
                var allValidEntries = Firstname.Text != "" && Lastname.Text != "" && 
                                      ValidationsHelper.IsStrongPassword(PasswordEntry.Text) && PasswordEntry.Text == ConfirmPasswordEntry.Text;

                AuthenticationPagesHelper.ToggleActionButtonButton(SignUpBtn, allValidEntries);
            }
        }

        private async void SignUpBtn_Clicked(object sender, EventArgs e)
        {
            KeyboardHelper.Dismiss();
            try
            {
                await AppLoader.ShowAsync("Creating your account and capturing your info...", 2);

                var registerData = new UserModel
                {
                    Email = LocalStorageService.UserDetails.Email,
                    CountryCode = !string.IsNullOrEmpty(PhoneNumber.Text) ? $"+{CountryCode.SelectedItem.ToString().Split("+")[1]}" : "+1",
                    Firstname = Firstname.Text,
                    Lastname = Lastname.Text,
                    Password = PasswordEntry.Text,
                    PhoneNumber = !string.IsNullOrEmpty(PhoneNumber.Text) ? PhoneNumber.Text : "",
                    UserRole = UserRoleType.Client
                };

                LocalStorageService.UserDetails = registerData;

                var bodyMetrics = LocalStorageService.UserBodyMetrics;

                DefaultValueHelper.SetDefaults(registerData);
                DefaultValueHelper.SetDefaults(LocalStorageService.UserBodyMetrics);
                DefaultValueHelper.SetDefaults(LocalStorageService.UserCycleProfile);
                DefaultValueHelper.SetDefaults(LocalStorageService.UserSubscription);

                LocalStorageService.UserSubscription.ActiveDate = DateTime.MinValue;
                LocalStorageService.UserSubscription.NextPaymentDate = DateTime.MinValue;
                LocalStorageService.UserSubscription.SubscriptionType = SubscriptionType.PremiumMonthly;
                LocalStorageService.UserSubscription.Status = StatusType.Pending;

                var platform = DeviceInfo.Platform;

                if (platform == DevicePlatform.Android)
                    LocalStorageService.UserSubscription.MobileDeviceType = MobileDeviceType.Android;
                else if (platform == DevicePlatform.iOS)
                    LocalStorageService.UserSubscription.MobileDeviceType = MobileDeviceType.IOS;
                else
                    LocalStorageService.UserSubscription.MobileDeviceType = MobileDeviceType.Unknown;
                /*
                (var price, var currency) = await _applePay.GetSubscriptionPrice();
                var amount = await _applePay.GetConvertedPriceForOvulaeZarOrUsd(price);
                */

                var refCodeEntered = !string.IsNullOrEmpty(ReferalCode.Text) ? $"ovios-{ReferalCode.Text.ToLower()}" : "";

                var payload = new UserFullProfileViewModel
                {
                    UserDetails = registerData,
                    UserBodyMetrics = LocalStorageService.UserBodyMetrics,
                    UserProfileCycle = LocalStorageService.UserCycleProfile,
                    UserSubscription = LocalStorageService.UserSubscription,
                    AffiliateJoinLink = LocalStorageService.AffiliateJoinCode,
                };

#if IOS
                payload.AffiliateJoinLink = refCodeEntered;
#endif
                payload.UserDetails.UserId = "";
                payload.UserBodyMetrics.Id = 0;
                payload.UserProfileCycle.Id = 0;
                payload.UserSubscription.Id = 0;

                var registerSuccess = await _userApi.CreateUserDetailsForJoiningOvulae(payload);

                if (registerSuccess.Success)
                {
                    var userId = registerSuccess.Message;
                    LocalStorageService.UserDetails.UserId = userId;
                    LocalStorageService.UserSubscription.UserId = userId;
                    LocalStorageService.UserBodyMetrics.UserId = userId;
                    LocalStorageService.UserCycleProfile.UserId = userId;
                    
                    await _usersServ.SaveLocalData();

                    var primaryGoalStr = LocalStorageService.UserCycleProfile.OvulaePrimaryGoal;
                    var appPrimaryGoal = primaryGoalStr != null ? EnumHelper.GetEnumValueFromName<ModuleType>(primaryGoalStr) : ModuleType.PeriodTracker;
                    
                    LocalStorageService.AppPrimaryGoal = appPrimaryGoal;
                    LocalStorageService.Authenticated = true;

#if ANDROID
                    await Shell.Current.GoToAsync(nameof(PaymentWallPage));
#elif IOS
                    await Shell.Current.GoToAsync(nameof(PaymentWalliOSPage));
#endif
                    await AppLoader.HideAsync();
                }
                else
                {
                    await AppLoader.HideAsync();
                    if (registerSuccess.ErrorTypes.Contains(ErrorTypes.AUTH_PHONE_NUMBER_EXIST))
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Sign Up Error", "This phone number is already registered in our system, please use another one."));
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Sign Up Error", "Could not create user account."));
                    }
                }
            }
            catch(Exception ex)
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Sign Up Error", $"Could not create user account."));
            }
            finally
            { 
                await AppLoader.HideAsync(); 
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if(PasswordEntry.IsPassword)
            {
                PasswordCover.Source = "eye_hide";
                PasswordCover2.Source = "eye_hide";
                PasswordEntry.IsPassword = false;
                ConfirmPasswordEntry.IsPassword = false;
            }
            else
            {
                PasswordCover.Source = "eye_show";
                PasswordCover2.Source = "eye_show";
                PasswordEntry.IsPassword = true;
                ConfirmPasswordEntry.IsPassword = true;
            }
        }
    }

}
