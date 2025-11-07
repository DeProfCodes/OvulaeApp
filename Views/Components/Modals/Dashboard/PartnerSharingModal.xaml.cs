
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Pages.Authentication;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Errors;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.Diet;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class PartnerSharingModal : ContentView
    {
        private IUsersApi _userApi;
        private ModuleType moduleType;
        private ModalCloseType modalCloseType;

        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;
        private string CurrentSlide;

        public PartnerSharingModal()
        {
            InitializeComponent();

            modalCloseType = ModalCloseType.None;
        }

        public Task<ModalCloseType> ShowPartnerSharingModal(ModuleType moduleType, IUsersApi userApi)
        {
            this.IsVisible = true;
            this.moduleType = moduleType;
            _userApi = userApi;

            CurrentSlide = "Intro";
            NextBtnTxt.Text = "Submit";
            BackBtnTxt.Text = "Close";

            InviteForm.IsVisible = true;
            LoadingContainer.IsVisible = false;
            InviteStatusContainer.IsVisible = false;
            BackBtn.IsVisible = false;
            SubTitle.IsVisible = true;

            NextBtnTxt.IsVisible = true;
            MainBodyContainer.HeightRequest = 350;

            _resultCompletionSource = new TaskCompletionSource<ModalCloseType>();

            this.FadeTo(1, 200);

            return _resultCompletionSource.Task;
        }

        public async Task HideAsync()
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;

            _resultCompletionSource?.TrySetResult(modalCloseType);
        }

        private async void CloseButton_Tapped(object sender, TappedEventArgs e)
        {
            KeyboardHelper.Dismiss();
            modalCloseType = (modalCloseType != ModalCloseType.None) ? modalCloseType : ModalCloseType.CloseButton;
            await HideAsync();
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                if (CurrentSlide == "Intro")
                {
                    modalCloseType = (modalCloseType != ModalCloseType.None) ? modalCloseType : ModalCloseType.Reject;
                    await HideAsync();
                }
                else if(CurrentSlide == "Submitting")
                {
                    InviteForm.IsVisible = true;
                    LoadingContainer.IsVisible = false;
                    InviteStatusContainer.IsVisible = false;
                    BackBtn.IsVisible = false;
                    SubTitle.IsVisible = true;

                    CurrentSlide = "Intro";
                    BackBtnTxt.Text = "Close";

                    NextBtnTxt.IsVisible = true;
                    NextBtnTxt.Text = "Submit";
                    MainBodyContainer.HeightRequest = 350;
                }
            }
            catch 
            {
                modalCloseType = ModalCloseType.Error;
                await HideAsync();
            }
        }

        private async void InvitePartnerBtnTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                if (CurrentSlide == "Submitting")
                {
                    await HideAsync();

                    return;
                }

                BackBtnTxt.Text = "Back";
                CurrentSlide = "Submitting";

                SubTitle.IsVisible = false;
                InviteForm.IsVisible = false;
                LoadingContainer.IsVisible = true;
                MainBodyContainer.HeightRequest = 230;

                var data = new UserPartnerViewModel
                {
                    Firstname = Firstname.Text.Trim(),
                    Lastname = Lastname.Text.Trim(),
                    Email = Email.Text.Trim(),
                    CountryCode = $"+{CountryCode.SelectedItem.ToString().Split("+")[1]}",
                    PhoneNumber = PhoneNumber.Text.Trim(),
                    MainUserId = LocalStorageService.UserDetails.UserId,
                    MainUserFullname = $"{LocalStorageService.UserDetails.Firstname} {LocalStorageService.UserDetails.Lastname}",
                    Journey = $"{moduleType.GetDisplayShortName()} Tracking"
                };

                var invitePartner = await _userApi.CreateUserPartner(data);

                LoadingContainer.IsVisible = false;
                InviteStatusContainer.IsVisible = true;

                if (invitePartner.Success)
                {
                    InviteStatusText.Text = $"You have successfully invited your partner. {invitePartner.Note}";
                    modalCloseType = ModalCloseType.Accept;

                    NextBtnTxt.Text = "Finish";

                    LocalStorageService.PartnerDetails = new UserPartnerViewModel
                    {
                        Firstname = data.Firstname,
                        Lastname = data.Lastname,
                        Email = data.Email,
                        CountryCode = data.CountryCode,
                        PhoneNumber = data.PhoneNumber,
                        Journey = data.Journey,
                        MainUserFullname = data.MainUserFullname,
                        MainUserId = LocalStorageService.UserDetails.UserId,
                        TempPassword = data.TempPassword,
                        InviteDate = DateTime.UtcNow,
                        AccessStatus = AccountStatusType.Active,
                        LastUpdateDate = DateTime.UtcNow
                    };
                }
                else
                {
                    if (invitePartner.ErrorTypes.Contains(ErrorTypes.AUTH_EMAIL_EXIST))
                    {
                        InviteStatusText.Text = $"Cannot invite as partner, There is already an active user with this email!";
                    }
                    else if (invitePartner.ErrorTypes.Contains(ErrorTypes.AUTH_PHONE_NUMBER_EXIST))
                    {
                        InviteStatusText.Text = $"Cannot invite as partner, There is already an active user with this phone number!";
                    }
                    else
                    {
                        InviteStatusText.Text = $"Something went wrong, couldn't invite your partner, please try again. {invitePartner.Note}";
                    }
                    modalCloseType = ModalCloseType.Reject;

                    BackBtn.IsVisible = true;
                    NextBtnTxt.Text = "Close";
                }
            }
            catch
            {
                modalCloseType = ModalCloseType.Error;
                await HideAsync();
            }
        }

        private void OnFieldsChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (sender == Email)
                {
                    EmailValidation.IsVisible = !ValidationsHelper.IsValidEmail(Email.Text);
                }
                else if (entry == Firstname)
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

                    bool isValid = digits.Length >= 8 && digits.Length <= 15;

                    PhoneNumberValidation.IsVisible = !isValid;
                }
            }

            if (Firstname.Text != null && Lastname.Text != null && PhoneNumber.Text != null)
            {
                var allValidEntries = Firstname.Text != "" && Lastname.Text != "" && (PhoneNumber.Text.Length >= 8 && PhoneNumber.Text.Length <= 15) &&
                                      ValidationsHelper.IsValidEmail(Email.Text);

                InvitePartnerBtn.Opacity = allValidEntries ? 1 : 0.3;
                InvitePartnerBtn.IsEnabled = allValidEntries;
            }
        }

    }
}