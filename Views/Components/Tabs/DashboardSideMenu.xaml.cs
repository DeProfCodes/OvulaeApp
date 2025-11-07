using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.Components.Modals.Dashboard;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.PrivacyPolicy;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.User;
using OvulaeShared.Services.APIs.ModuleServices;

namespace OvulaeApp.Views.Components.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardSideMenu : ContentView
    {
        //Services
        private IUserLocalService _userServ => ServiceHelper.GetService<IUserLocalService>();
        private IModuleLogsService _moduleLogsServ => ServiceHelper.GetService<IModuleLogsService>();

        //Loader Components
        private SpinnerLoader SpinnerLoader;
        private BrandedLoader AppLoader;
        private TrackerSwitcher TrackerSwitcher;

        //Modal Components
        private PregnancyCompleteModal PregnancyCompleteModal;
        private PregnancyTrackerOnboardModal PregnancyTrackerOnboardModal;
        private PeriodTrackerOnboardModal PeriodTrackerOnboardModal;
        private MenopauseTrackerOnboardModal MenopauseTrackerOnboardModal;

        //Popup Components
        private YesNoAlertModal YesNoAlertModal;

        public ICommand TapCommand { get; set; }

        public string AppVersion => $"Version {VersionTracking.CurrentVersion}";

        public bool IsMainUser { get; set; }

        public bool ShowSwitchToPregnancy { get; set; }
        public bool ShowSwitchToOvulation { get; set; }
        public bool ShowSwitchToPeriodTracker { get; set; }
        public bool ShowSwitchToMenopause { get; set; }

        public DashboardSideMenu()
        {
            InitializeComponent();

            //TapCommand = new Command<string>(OnMenuItemTapped);

            IsMainUser = LocalStorageService.UserDetails.UserRole == UserRoleType.Client;

            ShowSwitchToPregnancy = LocalStorageService.AppPrimaryGoal != ModuleType.Pregnancy;
            ShowSwitchToPeriodTracker = LocalStorageService.AppPrimaryGoal != ModuleType.PeriodTracker;
            ShowSwitchToOvulation = LocalStorageService.AppPrimaryGoal != ModuleType.Ovulation;
            ShowSwitchToMenopause = LocalStorageService.AppPrimaryGoal != ModuleType.MenopauseTracker;

            BindingContext = this;
        }

        public void SetSpinnerLoader(SpinnerLoader spinnerLoader)
        {
            SpinnerLoader = spinnerLoader;
        }

        public void SetAppLoader(BrandedLoader appLoader)
        {
            AppLoader = appLoader;
        }

        public void SetLoaders(SpinnerLoader spinnerLoader, BrandedLoader appLoader)
        {
            SpinnerLoader = spinnerLoader;
            AppLoader = appLoader;
        }

        public void SetTrackerSwitcherComponent(TrackerSwitcher trackerSwitcher)
        {
            TrackerSwitcher = trackerSwitcher;
        }

        public void SetModalComponets(PregnancyTrackerOnboardModal pregnancyTrackerOnboardModal, PeriodTrackerOnboardModal periodTrackerOnboardModal, 
                                      PregnancyCompleteModal pregnancyCompleteModal = null)
        {
            PregnancyTrackerOnboardModal = pregnancyTrackerOnboardModal;
            PeriodTrackerOnboardModal = periodTrackerOnboardModal;
            PregnancyCompleteModal = pregnancyCompleteModal;
        }

        public void ConfigureComponents(params object[] components)
        {
            foreach (var component in components)
            {
                switch (component)
                {
                    case SpinnerLoader spinner:
                        SpinnerLoader = spinner;
                        break;
                    case BrandedLoader appLoader:
                        AppLoader = appLoader;
                        break;
                    case TrackerSwitcher trackerSwitcher:
                        TrackerSwitcher = trackerSwitcher;
                        break;
                    case PregnancyTrackerOnboardModal pregnancyOnboard:
                        PregnancyTrackerOnboardModal = pregnancyOnboard;
                        break;
                    case PeriodTrackerOnboardModal periodOnboard:
                        PeriodTrackerOnboardModal = periodOnboard;
                        break;
                    case PregnancyCompleteModal pregnancyComplete:
                        PregnancyCompleteModal = pregnancyComplete;
                        break;
                    case MenopauseTrackerOnboardModal menopauseTrackerOnboard:
                        MenopauseTrackerOnboardModal = menopauseTrackerOnboard;
                        break;
                    case YesNoAlertModal yesNoAlert:
                        YesNoAlertModal = yesNoAlert;
                        break;
                }
            }
        }

        public async Task OpenAsync()
        {
            this.IsVisible = true;

            await Task.WhenAll(Overlay.FadeTo(1, 200, Easing.CubicInOut), SideMenuTray.TranslateTo(-1, 0, 250, Easing.CubicOut));
        }

        public async Task CloseAsync()
        {
            await Task.WhenAll(Overlay.FadeTo(0, 200, Easing.CubicInOut), SideMenuTray.TranslateTo(-250, 0, 200, Easing.CubicIn));
            this.IsVisible = false;
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await CloseAsync();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await CloseAsync();
        }

        private async Task NavigateToPage(string pageName)
        {
            await CloseAsync();

            var pageType = EnumHelper.GetEnumValueFromName<PageNameTypes>(pageName);

            if (AppLoader != null)
            {
                await AppLoader.ShowAsync(NavigationsHelper.GetLoaderMessage(pageType), NavigationsHelper.GetLoaderModalLines(pageType));
            }
            else if (SpinnerLoader != null)
                await SpinnerLoader.ShowSpinnerAsync();

            await Shell.Current.GoToAsync(pageName);

            if (AppLoader != null)
            {
                await AppLoader.HideAsync();
            }
            else if (SpinnerLoader != null)
                await SpinnerLoader.HideSpinnerAsync();
        }

        private async void LogoutClicked(object sender, EventArgs e)
        {
            await NavigateToPage(nameof(LoginPage));
        }

        private async void OnPrivacyTapped(object sender, EventArgs e)
        {
            await NavigateToPage(nameof(PrivacyPolicyPage));
        }

        private async void OnTermsTapped(object sender, EventArgs e)
        {
            await NavigateToPage(nameof(TermsOfUsePage));
        }

        private async void NavigationButtonClicked(object sender, TappedEventArgs e)
        {
            if (sender is Border border)
            {
                try
                {
                    if (e.Parameter is string pageParam)
                    {
                        SwitchTrackerGroup.IsVisible = false;
                        await VisualEventsHelper.TapDimEffectGray(border);

                        var moduleTabs = new List<string> { "Home", "Calendar" };

                        if(moduleTabs.Contains(pageParam)) 
                            pageParam = $"{LocalStorageService.AppPrimaryGoal.GetDisplayShortName()}{pageParam}";

                        var pageName = NavigationsHelper.GetCommonAppPagePageString(pageParam);

                        await NavigateToPage(pageName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in MenuItemBorder_Tapped: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to navigate.", "OK");
                }
            }
        }

        private async void SwitchTrackerTapped(object sender, TappedEventArgs e)
        {
            await VisualEventsHelper.TapDimEffectGray(SwitchTrackerBtn);
            
            SwitchTrackerGroup.IsVisible = !SwitchTrackerGroup.IsVisible;
        }

        private async void SwitchTrackerItemTapped(object sender, TappedEventArgs e)
        {
            if (sender is Border border)
            {
                try
                {
                    if (e.Parameter is string pageParam)
                    {
                        await VisualEventsHelper.TapDimEffectGray(border);

                        var newModule = EnumHelper.GetEnumValueFromName<ModuleType>(pageParam);
                        var oldTracker = LocalStorageService.AppPrimaryGoal;

                        var confirm = await YesNoAlertModal.ShowYesNoModal(
                                                                $"Switch Tracker",
                                                                $"Are you sure you want to switch your primary tracker from {oldTracker.GetDisplayShortName()} to {newModule.GetDisplayDescription()}"
                                                            );

                        if (confirm == ModalCloseType.Accept)
                        {
                            await CloseAsync();

                            if (oldTracker == ModuleType.Pregnancy)
                            {
                                var recordCompletePregnancy = await RecordCompletePregnancy();
                                if (!recordCompletePregnancy)
                                {
                                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Complete Pregnancy First", "Please note since you are switching from Pregnancy Tracker we require you to record your pregnancy completion."));
                                    return;
                                }
                            }

                            if (newModule == ModuleType.PeriodTracker || newModule == ModuleType.Ovulation)
                            {
                                var switchStatus = await HandlePeriodOvulationTrackerSwitch(newModule);
                                return;
                            }
                            else if (newModule == ModuleType.Pregnancy)
                            {
                                var switchStatus = await HandlePregnancyTrackerSwitch();
                            }
                            else if (newModule == ModuleType.MenopauseTracker)
                            {
                                var switchStatus = await HandleMenopauseTrackerSwitch();
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public async Task<bool> RecordCompletePregnancy()
        {
            try
            {
                var response = await PregnancyCompleteModal.ShowOnboardingModal();
                return (response == ModalCloseType.Accept);
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> HandlePeriodOvulationTrackerSwitch(ModuleType moduleType)
        {
            try
            {
                var trackerName = moduleType.GetDisplayDescription();
                var allSet = await PeriodTrackerOnboardModal.ShowOnboardingModal($"{trackerName} Setup");
                if (allSet == ModalCloseType.Accept)
                {
                    await TrackerSwitcher.ShowAsync(moduleType);

                    var dbUpdateStatus = await _userServ.UpdateAppPrimaryGoal(moduleType);

                    if (dbUpdateStatus)
                    {
                        if(moduleType == ModuleType.PeriodTracker)
                            await _moduleLogsServ.LoadPeriodLogs(LocalStorageService.UserDetails.UserId);
                        else
                            await _moduleLogsServ.LoadOvulationLogs(LocalStorageService.UserDetails.UserId);

                        await Shell.Current.GoToAsync(NavigationsHelper.GetDashboardPageNameFromModule(moduleType));
                        await TrackerSwitcher.HideAsync();
                        return true;
                    }
                    else
                    {
                        await TrackerSwitcher.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"Something went wrong while attempting to switch to {trackerName}. Please make sure you are connect to the internet and try again."));
                    }
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Not Set", $"Once you are ready to start tracking {moduleType.GetDisplayShortName()} cycle, just click this button again 😊"));
                }
            }
            catch
            {
                await TrackerSwitcher.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Internal Error", $"Something went wrong while attempting to switch to {moduleType.GetDisplayDescription()}. Please make sure you are connected to the internet and try again."));
            }
            return false;
        }

        public async Task<bool> HandlePregnancyTrackerSwitch()
        {
            try
            {
                var allSet = await PregnancyTrackerOnboardModal.ShowOnboardingModal();
                if (allSet == ModalCloseType.Accept)
                {
                    await TrackerSwitcher.ShowAsync(ModuleType.Pregnancy);

                    var dbUpdateStatus = await _userServ.UpdateAppPrimaryGoal(ModuleType.Pregnancy);

                    if (dbUpdateStatus)
                    {
                        await _moduleLogsServ.LoadPregnancyLogs(LocalStorageService.UserDetails.UserId);
                        await Shell.Current.GoToAsync(nameof(PregnancyDashboardHomePage));
                        await TrackerSwitcher.HideAsync();
                        return true;
                    }
                    else
                    {
                        await TrackerSwitcher.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"Something went wrong while attempting to switch to Pregnancy Tracker. Please make sure you are connected to the internet and try again."));
                    }
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Not Set", $"Once you are ready to start tracking Pregnancy, just click this button again 😊"));
                }
            }
            catch
            {
                await TrackerSwitcher.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Internal Error", $"Something went wrong while attempting to switch to Pregnancy Tracker. Please make sure you are connected to the internet and try again."));
            }
            return false;
        }

        public async Task<bool> HandleMenopauseTrackerSwitch()
        {
            try
            {
                var allSet = await MenopauseTrackerOnboardModal.ShowOnboardingModal();
                if (allSet == ModalCloseType.Accept)
                {
                    await TrackerSwitcher.ShowAsync(ModuleType.MenopauseTracker);

                    var dbUpdateStatus = await _userServ.UpdateAppPrimaryGoal(ModuleType.MenopauseTracker);

                    if (dbUpdateStatus)
                    {
                        await _moduleLogsServ.LoadMenopauseLogs(LocalStorageService.UserDetails.UserId);
                        await Shell.Current.GoToAsync(nameof(MenopauseDashboardHomePage));
                        await TrackerSwitcher.HideAsync();
                        return true;
                    }
                    else
                    {
                        await TrackerSwitcher.HideAsync();
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", $"Something went wrong while attempting to switch to Menopause Tracker. Please make sure you are connected to the internet and try again."));
                    }
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Not Set", $"Once you are ready to start tracking Menopause, just click this button again 😊"));
                }
            }
            catch
            {
                await TrackerSwitcher.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Internal Error", $"Something went wrong while attempting to switch to Menopause Tracker. Please make sure you are connected to the internet and try again."));
            }
            return false;
        }
    }
}
