using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.Settings;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.Status;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Affiliate;
using OvulaeShared.Services.APIs.Affiliates;
using OvulaeShared.ViewModel.Affiliates;

namespace OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings
{
    public partial class EditSettingsAffiliateInfoPage : ContentPage
    {
        private EditSettingsAffilateViewModel vm;

        private readonly IUserLocalService _usersServ;
        private IAffiliatesApi _affServ;

        private bool isUpdate;

        public EditSettingsAffiliateInfoPage(IUserLocalService usersServ, IAffiliatesApi affServ)
        {
            InitializeComponent();

            _usersServ = usersServ;
            _affServ = affServ;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                vm = new EditSettingsAffilateViewModel();
                BindingContext = vm;

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

                var affiliateData = LocalStorageService.AffiliateOverviewDetails;

                if (affiliateData == null)
                {
                    NoAffiliateOption.IsVisible = true;
                    ExistingAffiliateOption.IsVisible = false;
                    ApplicationSentAffiliate.IsVisible = false;
                }
                else
                {
                    NoAffiliateOption.IsVisible = false;
                    ExistingAffiliateOption.IsVisible = affiliateData.AffiliateProfile.ProfileStatus == StatusType.Active;
                    ApplicationSentAffiliate.IsVisible = affiliateData.AffiliateProfile.ProfileStatus == StatusType.Pending;
                }
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

        private void JoinAffiliateProgrammeTapped(object sender, EventArgs e)
        {
            ToggleAffiliateOptions();

            var isOpening = JoinAffiliatesBtn.Text == "Join Affiliate Programme";
            JoinAffiliatesBtn.Text = isOpening ? "Close Editor" : "Join Affiliate Programme";

            EditFormTitle.Text = "Affiliate Sign Up";
            EditFormTitle.TextTransform = TextTransform.Uppercase;

            JoinAffiliatesBtn.IsVisible = false;
            FormBorder.IsVisible = isOpening;
            AffiliateJoinSection.IsVisible = isOpening;
        }

        private void ToggleSocialSelected(AffiliateEntryViewModel affEntry)
        {
            if (affEntry.Handle != "")
            {
                AffiliateSocialsOptions.SelectValue(affEntry.Type);
            }
        }

        private void EditApplication_Tapped(object sender, EventArgs e)
        {
            var isOpening = EditApplicationBtn.Text == "Edit Your Application";

            EditApplicationBtn.Text = isOpening ? "Close Editor" : "Edit Your Application";

            EditFormTitle.Text = "Edit Affiliate Applicaiton";
            EditFormTitle.TextTransform = TextTransform.None;

            FormBorder.IsVisible = isOpening;
            AffiliateJoinSection.IsVisible = isOpening;

            SubmitJoinAffilate.IsVisible = isOpening;

            ToggleSocialSelected(vm.AffiliateFacebook);
            ToggleSocialSelected(vm.AffiliateYoutube);
            ToggleSocialSelected(vm.AffiliateTiktok);
            ToggleSocialSelected(vm.AffiliateInstagram);
            ToggleSocialSelected(vm.AffiliateX);
            ToggleAffiliateOptions();

            isUpdate = isOpening;
        }

        private void ToggleAffiliateOptions()
        {
            AffiliateFacebook.IsVisible = AffiliateSocialsOptions.SelectedValues.Contains("Facebook");
            AffiliateTiktok.IsVisible = AffiliateSocialsOptions.SelectedValues.Contains("Tiktok");
            AffiliateX.IsVisible = AffiliateSocialsOptions.SelectedValues.Contains("X");
            AffiliateInstagram.IsVisible = AffiliateSocialsOptions.SelectedValues.Contains("Instagram");
            AffiliateYouTube.IsVisible = AffiliateSocialsOptions.SelectedValues.Contains("YouTube");

            SubmitJoinAffilate.IsVisible = AffiliateFacebook.IsVisible || AffiliateTiktok.IsVisible || AffiliateX.IsVisible ||
                                           AffiliateInstagram.IsVisible || AffiliateYouTube.IsVisible;
        }

        private void AffiliateSocialChoicesChanged(object sender, EventArgs e)
        {
            ToggleAffiliateOptions();
        }

        private void EditFormCloseButtonTapped(object sender, EventArgs e)
        {
            JoinAffiliatesBtn.Text = "Join Affiliate Programme";
            EditApplicationBtn.Text = "Edit Your Application";

            JoinAffiliatesBtn.IsVisible = true;
            FormBorder.IsVisible = false;
        }

        private void ViewMoreAffiliateDetails(object sender, EventArgs e)
        {
            var isOpening = ViewMoreAffDetailsBtn.Text.ToLower().Contains("more");

            ViewMoreAffDetailsBtn.Text = isOpening ? "Show Less" : "View More Details";
            FormBorder.IsVisible = isOpening;
            EditFormTitle.Text = "Your Affiliate Details";
            AffiliateMoreDetails.IsVisible = isOpening;
        }

        private async Task<bool> IsValidAffiliateJoinForm()
        {
            if (isUpdate)
                return true;

            var errorMessage = "Please enter valid";
            if (AffiliateSocialsOptions.SelectedValues.Contains("Facebook"))
            {
                if (vm.AffiliateFacebook.Handle == "") errorMessage += " Facebook handle url,";
                if (vm.AffiliateFacebook.FollowersCount < 0) errorMessage += " Facebook followers count,";
            }
            if (AffiliateSocialsOptions.SelectedValues.Contains("YouTube"))
            {
                if (vm.AffiliateYoutube.Handle == "") errorMessage += " YouTube handle url,";
                if (vm.AffiliateYoutube.FollowersCount < 0) errorMessage += " YouTube followers count,";
            }
            if (AffiliateSocialsOptions.SelectedValues.Contains("Instagram"))
            {
                if (vm.AffiliateInstagram.Handle == "") errorMessage += " Instagram handle url,";
                if (vm.AffiliateInstagram.FollowersCount < 0) errorMessage += " Instagram followers count,";
            }
            if (AffiliateSocialsOptions.SelectedValues.Contains("Tiktok"))
            {
                if (vm.AffiliateTiktok.Handle == "") errorMessage += " Tiktok handle url,";
                if (vm.AffiliateTiktok.FollowersCount < 0) errorMessage += " Tiktok followers count,";
            }
            if (AffiliateSocialsOptions.SelectedValues.Contains("X"))
            {
                if (vm.AffiliateX.Handle == "") errorMessage += " X handle url,";
                if (vm.AffiliateX.FollowersCount < 0) errorMessage += " X followers count,";
            }

            if (errorMessage != "Please enter valid")
            {
                errorMessage = errorMessage.Substring(0, errorMessage.Length - 1);

                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Invalid Inputs", errorMessage));

                return false;
            }
            return true;
        }

        private async void SubmitJoinAffilate_Tapped(object sender, EventArgs e)
        {
            try
            {
                var isValidForm = await IsValidAffiliateJoinForm();

                if (!isValidForm) return;

                var saveQuestion = await YesNoModal.ShowYesNoModal("Become an Affiliate?", "Are you sure you want to join our affiliate programme?");
                if (saveQuestion == ModalCloseType.Accept)
                {
                    await AppLoader.ShowAsync("Submitting your information...");

                    var affiliateProfile = new AffiliateProfile
                    {
                        UserId = LocalStorageService.UserDetails.UserId,
                        UserEmail = LocalStorageService.UserDetails.Email,
                        FacebookHandleLink = AffiliateSocialsOptions.SelectedValues.Contains("Facebook") ? vm.AffiliateFacebook.Handle : "",
                        FacebookFollowers = AffiliateSocialsOptions.SelectedValues.Contains("Facebook") ? vm.AffiliateFacebook.FollowersCount : 0,
                        YouTubeHandleLink = AffiliateSocialsOptions.SelectedValues.Contains("YouTube") ? vm.AffiliateYoutube.Handle : "",
                        YouTubeFollowers = AffiliateSocialsOptions.SelectedValues.Contains("YouTube") ? vm.AffiliateYoutube.FollowersCount : 0,
                        InstagramHandleLink = AffiliateSocialsOptions.SelectedValues.Contains("Instagram") ? vm.AffiliateInstagram.Handle : "",
                        InstagramFollowers = AffiliateSocialsOptions.SelectedValues.Contains("Instagram") ? vm.AffiliateInstagram.FollowersCount : 0,
                        TiktokHandleLink = AffiliateSocialsOptions.SelectedValues.Contains("Tiktok") ? vm.AffiliateTiktok.Handle : "",
                        TiktokFollowers = AffiliateSocialsOptions.SelectedValues.Contains("Tiktok") ? vm.AffiliateTiktok.FollowersCount : 0,
                        XHandleLink = AffiliateSocialsOptions.SelectedValues.Contains("X") ? vm.AffiliateX.Handle : "",
                        XFollowers = AffiliateSocialsOptions.SelectedValues.Contains("X") ? vm.AffiliateX.FollowersCount : 0,
                        CreateDate = DateTime.UtcNow,
                        LastUpdateDate = DateTime.UtcNow,
                        ProfileStatus = StatusType.Pending,
                    };

                    var applied = await _affServ.ApplyForAffiliateProgramme(affiliateProfile);

                    if (applied.Success)
                    {
                        FormBorder.IsVisible = false;

                        NoAffiliateOption.IsVisible = false;
                        ExistingAffiliateOption.IsVisible = false;  
                        ApplicationSentAffiliate.IsVisible = true;

                        if (LocalStorageService.AffiliateOverviewDetails == null)
                            LocalStorageService.AffiliateOverviewDetails = DefaultValueHelper.CreateWithDefaults<AffiliateOverviewDetails>();

                        LocalStorageService.AffiliateOverviewDetails.AffiliateProfile = affiliateProfile;

                        await _usersServ.SaveLocalData();

                        vm.LoadData();

                        await AppLoader.HideAsync();

                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Application Sent", "Your application has been sent successfully. We will review it and get back to you via email.", "Ok"));
                    }
                    else
                    {
                        await AppLoader.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Application Failed", "We could not receieve your application, something went wrong.", "Ok"));
                    }
                }
            }
            catch
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Application Failed", "We could not receieve your application, something went wrong.", "Ok"));
            }
        }

        private async void GoToDashboardPage(object sender, EventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://portal.ovulae.com");
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to open link.", "OK");
            }
        }

        private void ShareReferalLinkTapped(object sender, string param)
        {

        }
    }
}
