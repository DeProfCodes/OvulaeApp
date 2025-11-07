using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.Authentication;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;

namespace OvulaeApp.Views.PregnancyTracker.Onboarding
{
    public partial class PregnancyOnboardFinishPage : ContentPage
    {
        public PregnancyOnboardFinishPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var startTime = DateTime.Now;

                await RunInitialPregnancySetupAsync();

                var timeElapsed = DateTime.Now - startTime;
                var minDuration = TimeSpan.FromSeconds(5);

                if (timeElapsed < minDuration)
                    await Task.Delay(minDuration - timeElapsed);

                await Shell.Current.GoToAsync(nameof(SignUpEmailPage));
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to load pregnancy finish page: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while finilizing information.", "OK");
            }
        }

        public static int CalculatePregnancyWeek(DateTime? dueDate, DateTime? lastPeriodDate)
        {
            DateTime today = DateTime.Today;
            DateTime startDate;

            if (lastPeriodDate.HasValue)
            {
                startDate = lastPeriodDate.Value;
            }
            else if (dueDate.HasValue)
            {
                startDate = dueDate.Value.AddDays(-280); 
            }
            else
            {
                return 0; 
            }

            int weeks = (int)((today - startDate).TotalDays / 7.0);
            return Math.Max(1, Math.Min(weeks, 42)); 
        }

        private void SaveResponses()
        {
            DateTime? dueDate = Preferences.ContainsKey("DueDate") ? Preferences.Get("DueDate", DateTime.Today) : null;
            DateTime? lmp = Preferences.ContainsKey("LastPeriodDate") ? Preferences.Get("LastPeriodDate", DateTime.Today) : null;

            //User Profile Cycle
            LocalStorageService.UserCycleProfile.EstimatedDueDate = dueDate ?? lmp.Value.AddDays(280);
            LocalStorageService.UserCycleProfile.LastPeriodDate = lmp ?? dueDate.Value.AddDays(-280);

        }

        private async Task RunInitialPregnancySetupAsync()
        {
            try
            {
                DateTime? dueDate = Preferences.ContainsKey("DueDate") ? Preferences.Get("DueDate", DateTime.Today) : null;
                DateTime? lmp = Preferences.ContainsKey("LastPeriodDate") ? Preferences.Get("LastPeriodDate", DateTime.Today) : null;

                int week = CalculatePregnancyWeek(dueDate, lmp);

                SaveResponses();

                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to save onboarding questions data: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while saving onboarding questions.", "OK");
            }
        }
    }
}
