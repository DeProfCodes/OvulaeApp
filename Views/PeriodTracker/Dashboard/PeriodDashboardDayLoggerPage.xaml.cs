using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging.Abstractions;
using OvulaeApp.Helpers.Pages.DayLogging;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.PeriodTracker;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers.DayLogging;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Shared.Logs;
using OvulaeShared.Services.Module.CycleServices;

namespace OvulaeApp.Views.PeriodTracker.Dashboard
{
    [QueryProperty(nameof(EntryId), "entryId")]
    public partial class PeriodDashboardDayLoggerPage : ContentPage
    {
        public int EntryId { get; set; }

        private readonly IModuleLogsService _moduleLogsService;
        private readonly IUserLocalService _userService;
        private readonly ICycleService _cycleService;
        
        private PeriodLoggerViewModel viewModel;

        public string DateString => $"Date: {DateTime.Now:dd/MM/yyyy}";

        public bool _isNavigating { get; set; }

        public PeriodDashboardDayLoggerPage(IModuleLogsService moduleLogsService, IUserLocalService userServ, ICycleService cycleService)
        {
            InitializeComponent();
            BindingContext = this;

            _moduleLogsService = moduleLogsService;
            _userService = userServ;
            _cycleService = cycleService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadExistingLog();
        }

        private void LoadExistingLog()
        {
            try
            {
                viewModel = new PeriodLoggerViewModel(_moduleLogsService);
                BindingContext = viewModel;

                if (EntryId > 0)
                {
                    viewModel.CurrentLogEntry = _moduleLogsService.GetPeriodLogByEntryId(EntryId);
                }

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
                );

                Header.SetLoaders(Spinner, AppLoader);
                Header.OpenSideMenuCommand = new Command(async () => await SideMenu.OpenAsync());
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
        
        private void LoadCurrentLogData()
        {
            try
            {
                var todayLog = viewModel.CurrentLogEntry;

                var lastPeriodDate = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;
                var daysSinceLastPeriod = (DateTime.Today - lastPeriodDate).TotalDays;
                var customLMP = daysSinceLastPeriod > 7;

                var hadBowelMovements = todayLog.HadBowelMovements != null ?
                                        (todayLog.HadBowelMovements.Value ? CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0] :
                                                                            CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[1]) : "";

                var isBreastExamDate = LocalStorageService.UserCycleProfile.LastBreastExamDate == null || 
                                       DateTime.Today.Date == LocalStorageService.UserCycleProfile.LastBreastExamDate.Value.Date;

                BreastExamContainer.IsVisible = isBreastExamDate;
                BreastExamTimeContainer.IsVisible = isBreastExamDate;

                // Populate all multi-select components
                DayLoggerHelper.PopulateMultiSelectComponent(MoodsComponent, PeriodTrackerDayLogItems.Moods, todayLog.Moods);
                DayLoggerHelper.PopulateMultiSelectComponent(SymptomsComponent, PeriodTrackerDayLogItems.Symptoms, todayLog.Symptoms);
                DayLoggerHelper.PopulateMultiSelectComponent(EmotionsComponent, PeriodTrackerDayLogItems.Emotions, todayLog.Emotions);
                DayLoggerHelper.PopulateMultiSelectComponent(CravingsComponent, PeriodTrackerDayLogItems.Cravings, todayLog.Cravings);
                DayLoggerHelper.PopulateMultiSelectComponent(LifestyleActivities, PeriodTrackerDayLogItems.LifestyleFactors, todayLog.LifestyleFactors);
                DayLoggerHelper.PopulateMultiSelectComponent(Intercourse, PeriodTrackerDayLogItems.IntercourseOptions, todayLog.IntercourseInfo);
                DayLoggerHelper.PopulateMultiSelectComponent(ContraceptionMethod, PeriodTrackerDayLogItems.ContraceptionMethods, todayLog.ContraceptionMethodUsedToday);
                DayLoggerHelper.PopulateMultiSelectComponent(PeriodTodayComponent, CycleTrackerDayLogItems.PeriodStartOptions, todayLog.HadBleeding ? CycleTrackerDayLogItems.PeriodStartOptions[0] : "");
                DayLoggerHelper.PopulateMultiSelectComponent(PeriodLMPComponent, CycleTrackerDayLogItems.PeriodTimingOptions, customLMP ? CycleTrackerDayLogItems.PeriodStartOptions[0] : "");
                DayLoggerHelper.PopulateMultiSelectComponent(BleedingPresenceComponent, CycleTrackerDayLogItems.BleedingPresenceOptions, todayLog.BleedingPresence);
                DayLoggerHelper.PopulateMultiSelectComponent(BloodColorComponent, CycleTrackerDayLogItems.BloodColorOptions, todayLog.BleedingColor);
                DayLoggerHelper.PopulateMultiSelectComponent(FlowIntensityComponent, CycleTrackerDayLogItems.FlowIntensityOptions, todayLog.FlowIntensity);
                DayLoggerHelper.PopulateMultiSelectComponent(PainLevelComponent, CycleTrackerDayLogItems.PainLevelOptions, todayLog.PainLevel);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsExistComponent, CycleTrackerDayLogItems.HadPeriodBowelMovementOptions, hadBowelMovements);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsRegularityComponent, CycleTrackerDayLogItems.PeriodBowelMovementIrregularityOptions, todayLog.BowelMovementsRegularity);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsFrequencyComponent, CycleTrackerDayLogItems.PeriodBowelMovementFrequencyOptions, todayLog.BowelMovementsFrequency);

                MedicationComponent?.LoadMedications(todayLog.Medications);

                MoodRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.MoodsRating);
                SymptomsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.SymptomsRating);
                EmotionsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.EmotionsRating);
                CravingsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.CravingsRating);
                LifestyleRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.LifestyleRating);
                IntercourseRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.IntercourseRating);
                BreastTendernessRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(todayLog.BreastTendernessRating);

                MoodNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.MoodsNotes);
                SymptomsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.SymptomsNotes);
                EmotionsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.EmotionsNotes);
                CravingsNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.CravingsNotes);
                LifestyleNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.LifestyleNotes);
                IntercourseNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.IntercourseNotes);
                MedicationNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.MedicationNotes);
                ContraceptionNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.ContraceptionsMethodNotes);
                BreastTendernessNotes.Text = DefaultValueHelper.GetStringValueOrDefault(todayLog.BreastTendernessNotes);

                ReflectionText.Text = todayLog.Notes;

                var isPeriodDay = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
                PeriodLMPComponent.IsVisible = isPeriodDay;
                PeriodFlowData.IsVisible = isPeriodDay;
                CustomLMPContainer.IsVisible = isPeriodDay && PeriodLMPComponent.SelectedItems.Contains("🗓️ Different date...");

                LoadPcosData(todayLog);
                LoadEndoData(todayLog);

                UpdateRatingContainersVisibility();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization error: {ex.Message}");
            }
        }

        private void UpdateDayNavigationButtons()
        {
            // Hide next day button if we're already at today
            NextDayBtn.IsVisible = viewModel.CurrentDate < DateTime.Today;

            // Always show previous day button (unless you want to limit how far back they can go)
            PrevDayBtn.IsVisible = true;
        }

        private async void NavigateDays_Tapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating) return;

            _isNavigating = true;

            try
            {
                await Spinner.ShowSpinnerAsync();

                if (sender is Border border)
                {
                    if (border == PrevDayBtnBrd)
                    {
                        viewModel.GoToPreviousDay();
                    }
                    else if (border == NextDayBtnBrd)
                    {
                        viewModel.GoToNextDay();
                    }

                    UpdateDayNavigationButtons();
                    LoadCurrentLogData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate days: {ex.Message}");
            }
            finally
            {
                await Spinner.HideSpinnerAsync();
                _isNavigating = false;
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

        private void UpdateLogEntryFromUI()
        {
            var todayLog = viewModel.CurrentLogEntry;

            // Update model from UI
            todayLog.Moods = MoodsComponent.SelectedItems.ToList();
            todayLog.Symptoms = SymptomsComponent.SelectedItems.ToList();

            if (ManualSymptomsSwitch.IsToggled && !string.IsNullOrEmpty(LogManualSymptomText.Text))
            {
                todayLog.Symptoms.Add(LogManualSymptomText.Text);
            }

            todayLog.Emotions = EmotionsComponent.SelectedItems.ToList();
            todayLog.Cravings = CravingsComponent.SelectedItems.ToList();
            todayLog.LifestyleFactors = LifestyleActivities.SelectedItems.ToList();
            todayLog.IntercourseInfo = Intercourse.SelectedItems.ToList();
            todayLog.Notes = ReflectionText.Text;
            todayLog.HadBleeding = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
            todayLog.BleedingColor = BloodColorComponent.SelectedItems.FirstOrDefault();
            todayLog.FlowIntensityOption = FlowIntensityComponent.SelectedItems.FirstOrDefault();
            todayLog.PainLevel = PainLevelComponent.SelectedItems.FirstOrDefault();
            todayLog.HadBleeding = PeriodTodayComponent.SelectedItems.Contains("✅ Yes");
            todayLog.BleedingColor = BloodColorComponent.SelectedItems.FirstOrDefault();
            todayLog.FlowIntensityOption = FlowIntensityComponent.SelectedItems.FirstOrDefault();
            todayLog.PainLevel = PainLevelComponent.SelectedItems.FirstOrDefault();
            todayLog.BleedingPresence = BleedingPresenceComponent.SelectedItems.FirstOrDefault();

            todayLog.HadBowelMovements = BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0]) ? true :
                                         BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[1]) ? false : (bool?)null;

            todayLog.BowelMovementsRegularity = BowelMovementsRegularityComponent.SelectedItems.FirstOrDefault();
            todayLog.BowelMovementsFrequency = BowelMovementsFrequencyComponent.SelectedItems.FirstOrDefault();
            todayLog.Medications = MedicationComponent?.Medications ?? new();

            todayLog.MoodsRating = MoodsComponent.SelectedItems.Count() > 0 ? MoodRating.SelectedRating : 0;
            todayLog.SymptomsRating = SymptomsComponent.SelectedItems.Count() > 0 ? SymptomsRating.SelectedRating : 0;
            todayLog.EmotionsRating = MoodsComponent.SelectedItems.Count() > 0 ? EmotionsRating.SelectedRating : 0;
            todayLog.CravingsRating = MoodsComponent.SelectedItems.Count() > 0 ? CravingsRating.SelectedRating : 0;
            todayLog.LifestyleRating = MoodsComponent.SelectedItems.Count() > 0 ? LifestyleRating.SelectedRating : 0;
            todayLog.IntercourseRating = MoodsComponent.SelectedItems.Count() > 0 ? IntercourseRating.SelectedRating : 0;
            todayLog.BreastTendernessRating = BreastTendernessRating.SelectedRating;

            todayLog.MoodsNotes = MoodNotes.Text;
            todayLog.SymptomsNotes = MoodNotes.Text;
            todayLog.EmotionsNotes = EmotionsNotes.Text;
            todayLog.CravingsNotes = CravingsNotes.Text;
            todayLog.LifestyleNotes = LifestyleNotes.Text;
            todayLog.IntercourseNotes = IntercourseNotes.Text;
            todayLog.MedicationNotes = MedicationNotes.Text;
            todayLog.ContraceptionsMethodNotes = ContraceptionNotes.Text;

            // Waist/Hip measurements
            if (double.TryParse(PcosWaistMeasurement.Text, out double pcosWaist))
                todayLog.PcosWaistMeasurement = pcosWaist;

            if (double.TryParse(PcosHipMeasurement.Text, out double pcosHip))
                todayLog.PcosHipMeasurement = pcosHip;

            if (double.TryParse(PcosWaistOnly.Text, out double pcosWaistOnly))
                todayLog.PcosWaistCircumference = pcosWaistOnly;

            // Weight tracking
            if (double.TryParse(PcosWeight.Text, out double pcosWeight))
                todayLog.PcosWeight = pcosWeight;

            todayLog.PcosWeightDate = PcosWeightDate.Date;
            todayLog.PcosWeightNotes = PcosWeightNotes.Text;

            // PCOS Symptoms
            todayLog.PcosSymptoms = PcosSymptomsComponent?.SelectedItems?.ToList() ?? new();
            todayLog.PcosAdditionalSymptomsNotes = PcosAdditionalSymptomsNotes.Text;
            todayLog.PcosSymptomsNotes = PcosAdditionalSymptomsNotes.Text; // Using same field for now
            todayLog.PcosSymptomsRating = PcosSymptomsComponent?.SelectedItems?.Any() == true ? 1 : 0; // Simple rating

            // =========== ENDOMETRIOSIS TRACKING ===========
            todayLog.EndoMedications = EndoMedicationComponent?.Medications ?? new();

            // Pain tracking
            todayLog.EndoPainRating = EndoPainRating.SelectedRating;
            todayLog.EndoPainComments = EndoPainComments.Text;
            todayLog.EndoPainLocation = EndoPainLocation.Text;
            todayLog.EndoPainDuration = EndoPainDuration.Text;

            // Waist/Hip measurements
            if (double.TryParse(EndoWaistMeasurement.Text, out double endoWaist))
                todayLog.EndoWaistMeasurement = endoWaist;

            if (double.TryParse(EndoHipMeasurement.Text, out double endoHip))
                todayLog.EndoHipMeasurement = endoHip;

            if (double.TryParse(EndoWaistOnly.Text, out double endoWaistOnly))
                todayLog.EndoWaistCircumference = endoWaistOnly;

            // Weight tracking
            if (double.TryParse(EndoWeight.Text, out double endoWeight))
                todayLog.EndoWeight = endoWeight;

            todayLog.EndoWeightDate = EndoWeightDate.Date;
            todayLog.EndoWeightNotes = EndoWeightNotes.Text;


            DefaultValueHelper.SetDefaults(todayLog);
        }

        private async void SaveTodaysLogs_Clicked(object sender, EventArgs e)
        {
            if (_isNavigating)
                return;

            _isNavigating = true;

            try
            {
                await AppLoader.ShowAsync("Saving your data...");

                UpdateLogEntryFromUI();

                // Save through viewModel
                var success = await viewModel.SaveCurrentLog();
                
                if (success)
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
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "An unexpected error occurred", "OK"));
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

        private async void ImTryingToConceiveTapped(object sender, TappedEventArgs e)
        {
            if (_isNavigating)
                return;

            _isNavigating = true;

            await SideMenu.HandlePeriodOvulationTrackerSwitch(ModuleType.Ovulation);

            _isNavigating = false;
        }

        private void BowelMovementsExistComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            var hasBowelMovements = BowelMovementsExistComponent.SelectedItems.Contains(CycleTrackerDayLogItems.HadPeriodBowelMovementOptions[0]);

            BowelMovementsRegularityComponent.IsVisible = hasBowelMovements;
            BowelMovementsFrequencyComponent.IsVisible = hasBowelMovements;
        }

        private void UpdateRatingContainersVisibility()
        {
            MoodRatingsContainer.IsVisible = MoodsComponent.SelectedItems.Count() > 0;
            SymptomsContainerRating.IsVisible = SymptomsComponent.SelectedItems.Count() > 0;
            EmotionsContainerRating.IsVisible = EmotionsComponent.SelectedItems.Count() > 0;
            CravingsContainerRating.IsVisible = CravingsComponent.SelectedItems.Count() > 0;
            LifestyleContainerRating.IsVisible = LifestyleActivities.SelectedItems.Count() > 0;
            IntercourseContainerRating.IsVisible = Intercourse.SelectedItems.Count() > 0;
            ContraceptionContainer.IsVisible = ContraceptionMethod.SelectedItems.Count() > 0;
        }

        private void SelectComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            UpdateRatingContainersVisibility();
        }

        private void TogglePcosSection_Tapped(object sender, EventArgs e)
        {
            if (PcosSectionContent.IsVisible)
            {
                PcosSectionContent.IsVisible = false;
                PcosSectionToggle.Text = "▼";
            }
            else
            {
                PcosSectionContent.IsVisible = true;
                PcosSectionToggle.Text = "▲";
            }
        }

        private void ToggleEndoSection_Tapped(object sender, EventArgs e)
        {
            if (EndoSectionContent.IsVisible)
            {
                EndoSectionContent.IsVisible = false;
                EndoSectionToggle.Text = "▼";
            }
            else
            {
                EndoSectionContent.IsVisible = true;
                EndoSectionToggle.Text = "▲";
            }
        }

        private void LoadPcosData(PeriodLogEntry logEntry)
        {
            try
            {
                // PCOS Medication
                DayLoggerHelper.PopulateMedicationComponent(PcosMedicationComponent, logEntry.PcosMedications);

                // Waist/Hip Measurements
                PcosWaistMeasurement.Text = logEntry.PcosWaistMeasurement > 0 ? logEntry.PcosWaistMeasurement.ToString() : "";
                PcosHipMeasurement.Text = logEntry.PcosHipMeasurement > 0 ? logEntry.PcosHipMeasurement.ToString() : "";
                PcosWaistOnly.Text = logEntry.PcosWaistCircumference > 0 ? logEntry.PcosWaistCircumference.ToString() : "";

                // Weight Tracking
                PcosWeight.Text = logEntry.PcosWeight > 0 ? logEntry.PcosWeight.ToString() : "";
                PcosWeightDate.Date = logEntry.PcosWeightDate != default ? logEntry.PcosWeightDate : DateTime.Today;
                PcosWeightNotes.Text = logEntry.PcosWeightNotes ?? "";

                // PCOS Symptoms
                DayLoggerHelper.PopulateMultiSelectComponent(PcosSymptomsComponent,
                    GetPcosSymptomsList(), logEntry.PcosSymptoms);

                // Additional Symptoms Notes
                PcosAdditionalSymptomsNotes.Text = logEntry.PcosAdditionalSymptomsNotes ?? "";
                PcosAdditionalSymptomsContainer.IsVisible = !string.IsNullOrEmpty(logEntry.PcosAdditionalSymptomsNotes);

                // Calculate ratio
                CalculateWaistHipRatios();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading PCOS data: {ex.Message}");
            }
        }

        private void LoadEndoData(PeriodLogEntry logEntry)
        {
            try
            {
                // Endo Medication
                DayLoggerHelper.PopulateMedicationComponent(EndoMedicationComponent, logEntry.EndoMedications);

                // Pain Tracking
                EndoPainRating.SelectedRating = logEntry.EndoPainRating;
                EndoPainComments.Text = logEntry.EndoPainComments ?? "";
                EndoPainLocation.Text = logEntry.EndoPainLocation ?? "";
                EndoPainDuration.Text = logEntry.EndoPainDuration ?? "";

                // Waist/Hip Measurements
                EndoWaistMeasurement.Text = logEntry.EndoWaistMeasurement > 0 ? logEntry.EndoWaistMeasurement.ToString() : "";
                EndoHipMeasurement.Text = logEntry.EndoHipMeasurement > 0 ? logEntry.EndoHipMeasurement.ToString() : "";
                EndoWaistOnly.Text = logEntry.EndoWaistCircumference > 0 ? logEntry.EndoWaistCircumference.ToString() : "";

                // Weight Tracking
                EndoWeight.Text = logEntry.EndoWeight > 0 ? logEntry.EndoWeight.ToString() : "";
                EndoWeightDate.Date = logEntry.EndoWeightDate != default ? logEntry.EndoWeightDate : DateTime.Today;
                EndoWeightNotes.Text = logEntry.EndoWeightNotes ?? "";

                // Calculate ratio
                CalculateWaistHipRatios();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Endo data: {ex.Message}");
            }
        }

        private void CalculateWaistHipRatios()
        {
            // PCOS Ratio Calculation
            if (double.TryParse(PcosWaistMeasurement.Text, out double pcosWaist) &&
                double.TryParse(PcosHipMeasurement.Text, out double pcosHip) &&
                pcosHip > 0)
            {
                double ratio = Math.Round(pcosWaist / pcosHip, 2);
                PcosWaistHipRatio.Text = $"Waist-to-Hip Ratio: {ratio}";
                PcosWaistHipRatio.TextColor = ratio > 0.85 ? Color.FromArgb("#FF0000") : Color.FromArgb("#008000");
            }
            else
            {
                PcosWaistHipRatio.Text = "Ratio: --";
            }

            // Endo Ratio Calculation
            if (double.TryParse(EndoWaistMeasurement.Text, out double endoWaist) &&
                double.TryParse(EndoHipMeasurement.Text, out double endoHip) &&
                endoHip > 0)
            {
                double ratio = Math.Round(endoWaist / endoHip, 2);
                EndoWaistHipRatio.Text = $"Waist-to-Hip Ratio: {ratio}";
                EndoWaistHipRatio.TextColor = ratio > 0.85 ? Color.FromArgb("#FF0000") : Color.FromArgb("#008000");
            }
            else
            {
                EndoWaistHipRatio.Text = "Ratio: --";
            }
        }

        // Helper method for PCOS symptoms list
        private List<string> GetPcosSymptomsList()
        {
            return new List<string>
            {
                "Irregular periods",
                "Heavy bleeding",
                "Acne",
                "Oily skin",
                "Weight gain",
                "Difficulty losing weight",
                "Male-pattern hair growth",
                "Thinning hair",
                "Skin darkening",
                "Skin tags",
                "Headaches",
                "Mood changes",
                "Pelvic pain",
                "Sleep problems"
            };
        }
    }
}
