using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.MenopauseTracker.Onboarding;
using OvulaeApp.Views.OvulationTracker.Onboarding;
using OvulaeApp.Views.PeriodTracker.Onboarding;
using OvulaeApp.Views.PregnancyTracker.Onboarding;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.User;

namespace OvulaeApp.Views.GoalSetting
{
    public partial class GoalSettingPage : ContentPage
    {
        public GoalSettingPage()
        {
            InitializeComponent();

            InitiateLocalStorageValues();
        }

        private void InitiateLocalStorageValues()
        {
            LocalStorageService.UserDetails = DefaultValueHelper.CreateWithDefaults<UserModel>();
            LocalStorageService.UserCycleProfile = DefaultValueHelper.CreateWithDefaults<UserCycleProfile>();
            LocalStorageService.UserSubscription = DefaultValueHelper.CreateWithDefaults<UserSubscription>();
            LocalStorageService.UserBodyMetrics = DefaultValueHelper.CreateWithDefaults<UserBodyMetric>();

            if (LocalStorageService.UserCycleProfile.AllOvulaeGoals == null)
            {
                LocalStorageService.UserCycleProfile.AllOvulaeGoals = new List<string>();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            //Shell.Current.GoToAsync(nameof(GoalSettingPage));
            return base.OnBackButtonPressed();
        }

        private async void PregnancyTrackerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                LocalStorageService.OnboardingData.Goals.Add(ModuleType.Pregnancy);
                LocalStorageService.UserCycleProfile.OvulaePrimaryGoal = ModuleType.Pregnancy.GetDisplayName();
                LocalStorageService.UserCycleProfile.AllOvulaeGoals.Add(ModuleType.Pregnancy.GetDisplayName());

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(PregnancyOnboardWelcomePage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void OvulationTrackerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                LocalStorageService.OnboardingData.Goals.Add(ModuleType.Ovulation);
                LocalStorageService.UserCycleProfile.OvulaePrimaryGoal = ModuleType.Ovulation.GetDisplayName();
                LocalStorageService.UserCycleProfile.AllOvulaeGoals.Add(ModuleType.Ovulation.GetDisplayName());

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(OvulationOnboardWelcomePage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void PeriodTrackerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                LocalStorageService.OnboardingData.Goals.Add(ModuleType.PeriodTracker);
                LocalStorageService.UserCycleProfile.OvulaePrimaryGoal = ModuleType.PeriodTracker.GetDisplayName();
                LocalStorageService.UserCycleProfile.AllOvulaeGoals.Add(ModuleType.PeriodTracker.GetDisplayName());

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(PeriodOnboardWelcomePage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }

        private async void MenopauseTrackerTapped(object sender, TappedEventArgs e)
        {
            try
            {
                LocalStorageService.OnboardingData.Goals.Add(ModuleType.MenopauseTracker);
                LocalStorageService.UserCycleProfile.OvulaePrimaryGoal = ModuleType.MenopauseTracker.GetDisplayName();
                LocalStorageService.UserCycleProfile.AllOvulaeGoals.Add(ModuleType.MenopauseTracker.GetDisplayName());

                await Spinner.ShowSpinnerAsync();
                await Shell.Current.GoToAsync(nameof(MenopauseOnboardWelcomePage));
                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating.", "OK");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
            }
        }
    }
}
