using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Pages.DayLogging;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers.DayLogging;
using OvulaeShared.Models.Ovulation;

namespace OvulaeApp.Views.OvulationTracker.Dashboard
{
    public partial class OvulationDashboardDayLoggerPage : ContentPage
    {
        private readonly IModuleLogsService _moduleLogsService;
        private readonly IUserLocalService _userService;
        private readonly ICycleService _cycleService;

        private OvulationCycleLog todayLog;

        public string DateString => $"Date: {DateTime.Now:dd/MM/yyyy}";

        public bool _isNavigating { get; set; }

        public OvulationDashboardDayLoggerPage(IModuleLogsService moduleLogsService, IUserLocalService userService, ICycleService cycleService)
        {
            InitializeComponent();
            BindingContext = this;

            _moduleLogsService = moduleLogsService;
            _userService = userService;
            _cycleService = cycleService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadExistingLog();
            InitializeComponents();
        }

        private void LoadExistingLog()
        {
            try
            {
                todayLog = _moduleLogsService.GetTodayOvulationLog();
                if (todayLog == null)
                {
                    todayLog = DefaultValueHelper.CreateWithDefaults<OvulationCycleLog>();
                    todayLog.PhaseName = _cycleService.GetCurrentPhase().GetDisplayName();
                    todayLog.LogDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading log: {ex.Message}");
                Shell.Current.CurrentPage.ShowPopup(new BrandedAlertPopup("Error", "Failed to load your data", "OK"));
            }
        }

        private string GetClosestPeriodOption(DateTime lastPeriodDate)
        {
            var daysDiff = (DateTime.Today - lastPeriodDate).TotalDays;

            return daysDiff switch
            {
                <= 1 => "🕛 Today",
                <= 2 => "⏪ Yesterday",
                <= 3 => "⏪⏪ 2 Days Ago",
                _ => "🗓️ Different date..."
            };
        }

        private void InitializeComponents()
        {
            try
            {
                var lastPeriodDate = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                var daysSinceLastPeriod = (DateTime.Today - lastPeriodDate).TotalDays;
                var customLMP = daysSinceLastPeriod > 7;

                // Populate all multi-select components
                DayLoggerHelper.PopulateMultiSelectComponent(MoodsComponent, OvulationDayLogItems.Moods, todayLog.Moods);
                DayLoggerHelper.PopulateMultiSelectComponent(SymptomsComponent, PeriodTrackerDayLogItems.Symptoms, todayLog.Symptoms);
                DayLoggerHelper.PopulateMultiSelectComponent(EmotionsComponent, PeriodTrackerDayLogItems.Emotions, todayLog.Emotions);
                DayLoggerHelper.PopulateMultiSelectComponent(CravingsComponent, PeriodTrackerDayLogItems.Cravings, todayLog.Cravings);
                DayLoggerHelper.PopulateMultiSelectComponent(EnergyComponent, OvulationDayLogItems.EnergyLevels, todayLog.EnergyLevel);
                DayLoggerHelper.PopulateMultiSelectComponent(LifestyleComponent, OvulationDayLogItems.LifestyleActivities, todayLog.LifestyleFactors);
                DayLoggerHelper.PopulateMultiSelectComponent(CervicalMucusComponent, OvulationDayLogItems.CervicalMucusTypes, todayLog.CervicalMucusType);
                DayLoggerHelper.PopulateMultiSelectComponent(LHTestComponent, OvulationDayLogItems.LHTestResults, todayLog.LHTestResult);
                DayLoggerHelper.PopulateMultiSelectComponent(IntercourseComponent, OvulationDayLogItems.Intercourse, todayLog.IntercourseTiming);
                DayLoggerHelper.PopulateMultiSelectComponent(ContraceptionComponent, OvulationDayLogItems.ContraceptionMethods, todayLog.ContraceptiveMethod);
                DayLoggerHelper.PopulateMultiSelectComponent(CervixPositionComponent, OvulationDayLogItems.CervixPositionOptions, todayLog.CervixPosition);
                DayLoggerHelper.PopulateMultiSelectComponent(CervixFeelComponent, OvulationDayLogItems.CervixFeelOptions, todayLog.CervixFeel);
                DayLoggerHelper.PopulateMultiSelectComponent(PeriodTodayComponent, CycleTrackerDayLogItems.PeriodStartOptions, todayLog.HadBleeding ? CycleTrackerDayLogItems.PeriodStartOptions[0] : "");
                DayLoggerHelper.PopulateMultiSelectComponent(PeriodLMPComponent, CycleTrackerDayLogItems.PeriodTimingOptions, customLMP ? CycleTrackerDayLogItems.PeriodStartOptions[0] : "");
                DayLoggerHelper.PopulateMultiSelectComponent(BleedingPresenceComponent, CycleTrackerDayLogItems.BleedingPresenceOptions, todayLog.BleedingPresence);
                DayLoggerHelper.PopulateMultiSelectComponent(BloodColorComponent, CycleTrackerDayLogItems.BloodColorOptions, todayLog.BleedingColor);
                DayLoggerHelper.PopulateMultiSelectComponent(FlowIntensityComponent, CycleTrackerDayLogItems.FlowIntensityOptions, todayLog.FlowIntensity);
                DayLoggerHelper.PopulateMultiSelectComponent(PainLevelComponent, CycleTrackerDayLogItems.PainLevelOptions, todayLog.PainLevel);

                var hadBowelMovements = todayLog.HadBowelMovements != null ?
                                        (todayLog.HadBowelMovements.Value ? CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0] :
                                                                            CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[1]) : "";
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsExistComponent, CycleTrackerDayLogItems.HadPeriodBowelMovementOptions, hadBowelMovements);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsRegularityComponent, CycleTrackerDayLogItems.PeriodBowelMovementIrregularityOptions, todayLog.BowelMovementsRegularity);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsFrequencyComponent, CycleTrackerDayLogItems.PeriodBowelMovementFrequencyOptions, todayLog.BowelMovementsFrequency);

                // Set ratings
                MoodRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.MoodsRating);
                SymptomsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.SymptomsRating);
                EmotionsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.EmotionsRating);
                CravingsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.CravingsRating);
                EnergyRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.EnergyRating);
                LifestyleRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.LifestyleRating);
                BreastTendernessRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.BreastTendernessRating);

                // Set notes
                MoodNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.MoodsNotes);
                SymptomsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.SymptomsNotes);
                EmotionsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.EmotionsNotes);
                CravingsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.CravingsNotes);
                EnergyNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.EnergyNotes);
                LifestyleNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.LifestyleNotes);
                BreastTendernessNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.BreastTendernessNotes);

                ReflectionText.Text = todayLog.Notes;

                var isPeriodDay = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
                PeriodLMPComponent.IsVisible = isPeriodDay;
                PeriodFlowData.IsVisible = isPeriodDay;
                CustomLMPContainer.IsVisible = isPeriodDay && PeriodLMPComponent.SelectedItems.Contains("🗓️ Different date...");

                // Initialize loaders
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup, MenopauseTrackerOnBoard);
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization error: {ex.Message}");
            }
        }

        private void PeriodTodayComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            var isPeriodDay = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
            PeriodLMPComponent.IsVisible = isPeriodDay;
            PeriodFlowData.IsVisible = isPeriodDay;
        }

        private void PeriodLMPComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            CustomLMPContainer.IsVisible = PeriodLMPComponent.SelectedItems.Contains("🗓️ Different date...");
        }

        private void EnterSymptomsManuallySwitch_Toggled(object sender, EventArgs e)
        {
            LogManualSymptom.IsVisible = ManualSymptomsSwitch.IsToggled;
        }

        private async void SaveTodaysLogs_Clicked(object sender, EventArgs e)
        {
            if (_isNavigating)
                return;

            _isNavigating = true;

            try
            {
                await AppLoader.ShowAsync("Saving your data...");

                // Update model from UI
                todayLog.Moods = MoodsComponent.SelectedItems.ToList();
                todayLog.Symptoms = SymptomsComponent.SelectedItems.ToList();

                if (ManualSymptomsSwitch.IsToggled && !string.IsNullOrEmpty(LogManualSymptomText.Text))
                {
                    todayLog.Symptoms.Add(LogManualSymptomText.Text);
                }

                todayLog.Emotions = EmotionsComponent.SelectedItems.ToList();
                todayLog.Cravings = CravingsComponent.SelectedItems.ToList();
                todayLog.LifestyleFactors = LifestyleComponent.SelectedItems.ToList();
                todayLog.CervicalMucusType = CervicalMucusComponent.SelectedItems.FirstOrDefault();
                todayLog.LHTestResult = LHTestComponent.SelectedItems.FirstOrDefault();
                todayLog.IntercourseTiming = IntercourseComponent.SelectedItems.FirstOrDefault();
                todayLog.EnergyLevel = EnergyComponent.SelectedItems.FirstOrDefault();
                todayLog.ContraceptiveMethod = ContraceptionComponent.SelectedItems.FirstOrDefault();
                todayLog.CervixPosition = CervixPositionComponent.SelectedItems.FirstOrDefault();
                todayLog.CervixFeel = CervixFeelComponent.SelectedItems.FirstOrDefault();
                todayLog.Notes = ReflectionText.Text;
                todayLog.HadBleeding = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
                todayLog.BleedingColor = BloodColorComponent.SelectedItems.FirstOrDefault();
                todayLog.FlowIntensityOption = FlowIntensityComponent.SelectedItems.FirstOrDefault();
                todayLog.PainLevel = PainLevelComponent.SelectedItems.FirstOrDefault();
                todayLog.BleedingPresence = BleedingPresenceComponent.SelectedItems.FirstOrDefault();

                todayLog.HadBowelMovements = BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0]) ? true :
                                             BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[1]) ? false : (bool?)null;

                todayLog.BowelMovementsRegularity = BowelMovementsRegularityComponent.SelectedItems.FirstOrDefault();
                todayLog.BowelMovementsFrequency = BowelMovementsFrequencyComponent.SelectedItems.FirstOrDefault();

                // Set ratings
                todayLog.MoodsRating = MoodsComponent.SelectedItems.Count() > 0 ? MoodRating.SelectedRating : 0;
                todayLog.SymptomsRating = SymptomsComponent.SelectedItems.Count() > 0 ? SymptomsRating.SelectedRating : 0;
                todayLog.EmotionsRating = EmotionsComponent.SelectedItems.Count() > 0 ? EmotionsRating.SelectedRating : 0;
                todayLog.CravingsRating = CravingsComponent.SelectedItems.Count() > 0 ? CravingsRating.SelectedRating : 0;
                todayLog.EnergyRating = EnergyComponent.SelectedItems.Count() > 0 ? EnergyRating.SelectedRating : 0;
                todayLog.LifestyleRating = LifestyleComponent.SelectedItems.Count() > 0 ? LifestyleRating.SelectedRating : 0;
                todayLog.BreastTendernessRating = BreastTendernessRating.SelectedRating;

                // Set notes
                todayLog.MoodsNotes = MoodNotes.Text;
                todayLog.SymptomsNotes = SymptomsNotes.Text;
                todayLog.EmotionsNotes = EmotionsNotes.Text;
                todayLog.CravingsNotes = CravingsNotes.Text;
                todayLog.EnergyNotes = EnergyNotes.Text;
                todayLog.LifestyleNotes = LifestyleNotes.Text;
                todayLog.BreastTendernessNotes = BreastTendernessNotes.Text;

                DefaultValueHelper.SetDefaults(todayLog);

                // Save to database
                var result = await _moduleLogsService.UpdateTodayOvulationLog(todayLog);

                if (result.Success)
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your data was saved successfully!", "OK"));
                }
                else
                {
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Failed to save your data", "OK"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
                await Shell.Current.CurrentPage.ShowPopupAsync(
                    new BrandedAlertPopup("Error", "An unexpected error occurred", "OK"));
            }
            finally
            {
                await AppLoader.HideAsync();
                _isNavigating = false;
            }
        }

        private async void ImPregnantTapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating)
                return;

            _isNavigating = true;

            await SideMenu.HandlePregnancyTrackerSwitch();

            _isNavigating = false;
        }

        private void BowelMovementsExistComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            var hasBowelMovements = BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0]);

            BowelMovementsRegularityComponent.IsVisible = hasBowelMovements;
            BowelMovementsFrequencyComponent.IsVisible = hasBowelMovements;
        }

        private void SelectComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            MoodRatingsContainer.IsVisible = MoodsComponent.SelectedItems.Count() > 0;
            SymptomsContainerRating.IsVisible = SymptomsComponent.SelectedItems.Count() > 0;
            EmotionsContainerRating.IsVisible = EmotionsComponent.SelectedItems.Count() > 0;
            CravingsContainerRating.IsVisible = CravingsComponent.SelectedItems.Count() > 0;
            EnergyContainerRating.IsVisible = EnergyComponent.SelectedItems.Count() > 0;
            LifestyleContainerRating.IsVisible = LifestyleComponent.SelectedItems.Count() > 0;
            //IntercourseContainerRating.IsVisible = IntercourseComponent.SelectedItems.Count() > 0;
        }
    }
}