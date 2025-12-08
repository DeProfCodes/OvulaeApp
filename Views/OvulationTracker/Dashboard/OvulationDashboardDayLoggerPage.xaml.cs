using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Functions.Modules;
using OvulaeApp.Helpers.Pages.DayLogging;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.OvulationTracker;
using OvulaeApp.Views.Components.Dashboard;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers.DayLogging;
using OvulaeShared.Models.Ovulation;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Shared.Logs;
using OvulaeShared.Services.Module.CycleServices;

namespace OvulaeApp.Views.OvulationTracker.Dashboard
{
    [QueryProperty(nameof(EntryId), "entryId")]
    public partial class OvulationDashboardDayLoggerPage : ContentPage
    {
        public int EntryId { get; set; }

        private readonly IModuleLogsService _moduleLogsService;
        private readonly IUserLocalService _userService;
        private readonly ICycleService _cycleService;

        private OvulationLoggerViewModel viewModel;

        public bool _isNavigating { get; set; }

        public OvulationDashboardDayLoggerPage(IModuleLogsService moduleLogsService, IUserLocalService userService, ICycleService cycleService)
        {
            InitializeComponent();
            BindingContext = this;

            _moduleLogsService = moduleLogsService;
            _userService = userService;
            _cycleService = cycleService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadExistingLog();
        }

        private async Task LoadExistingLog()
        {
            try
            {
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup, MenopauseTrackerOnBoard);
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                // Initialize view model
                viewModel = new OvulationLoggerViewModel(_moduleLogsService);
                BindingContext = viewModel;

                await Spinner.ShowSpinnerAsync();

                if (EntryId > 0)
                {
                    await _moduleLogsService.LoadOvulationLogs(LocalStorageService.UserDetails.UserId);
                    viewModel.CurrentLogEntry = _moduleLogsService.GetOvulationLogByEntryId(EntryId);
                    viewModel.CurrentDate = viewModel.CurrentLogEntry?.LogDate ?? DateTime.Today;
                }

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading log: {ex.Message}");
                Shell.Current.CurrentPage.ShowPopup(new BrandedAlertPopup("Error", "Failed to load your data", "OK"));

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
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

                MedicationComponent?.LoadMedications(todayLog.Medications);

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
                MedicationNotes.Text = todayLog.MedicationNotes;

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

        private List<MedicationModel> GetMedicationsFromComponent(MedicationEntryComponent component)
        {
            if (component == null) return new List<MedicationModel>();

            var result = new List<MedicationModel>();

            foreach (var entry in component.MedicationEntries)
            {
                var name = entry.MedicationName?.Trim();
                var dosage = entry.Dosage?.Trim();

                if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(dosage))
                {
                    result.Add(new MedicationModel
                    {
                        Name = name ?? string.Empty,
                        Dosage = dosage ?? string.Empty
                    });
                }
            }

            return result;
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
            todayLog.Medications = MedicationComponent?.Medications ?? new();

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
            todayLog.MedicationNotes = MedicationNotes.Text;

            // Get medications DIRECTLY from entries (bypassing the property)
            todayLog.Medications = GetMedicationsFromComponent(MedicationComponent);
            todayLog.PcosMedications = GetMedicationsFromComponent(PcosMedicationComponent);
            todayLog.EndoMedications = GetMedicationsFromComponent(EndoMedicationComponent);

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
        }

        private async void SaveTodaysLogs_Clicked(object sender, EventArgs e)
        {
            if (_isNavigating)
                return;

            _isNavigating = true;

            try
            {
                await AppLoader.ShowAsync("Saving your data...");

                // Update the viewModel's CurrentLogEntry from UI
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

        private void UpdateRatingContainersVisibility()
        {
            MoodRatingsContainer.IsVisible = MoodsComponent.SelectedItems.Count() > 0;
            SymptomsContainerRating.IsVisible = SymptomsComponent.SelectedItems.Count() > 0;
            EmotionsContainerRating.IsVisible = EmotionsComponent.SelectedItems.Count() > 0;
            CravingsContainerRating.IsVisible = CravingsComponent.SelectedItems.Count() > 0;
            EnergyContainerRating.IsVisible = EnergyComponent.SelectedItems.Count() > 0;
            LifestyleContainerRating.IsVisible = LifestyleComponent.SelectedItems.Count() > 0;
            //IntercourseContainerRating.IsVisible = IntercourseComponent.SelectedItems.Count() > 0;
        }

        private void SelectComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            UpdateRatingContainersVisibility();
        }

        private void LoadPcosData(OvulationCycleLog logEntry)
        {
            try
            {
                var allLogs = _moduleLogsService.GetAllPeriodLogs();

                // PCOS Medication
                DayLoggerHelper.PopulateMedicationComponent(PcosMedicationComponent, logEntry.PcosMedications);

                // Waist/Hip Measurements
                PcosWaistMeasurement.Text = logEntry.PcosWaistMeasurement > 0 ? logEntry.PcosWaistMeasurement.ToString() : "";
                PcosHipMeasurement.Text = logEntry.PcosHipMeasurement > 0 ? logEntry.PcosHipMeasurement.ToString() : "";
                PcosWaistOnly.Text = logEntry.PcosWaistCircumference > 0 ? logEntry.PcosWaistCircumference.ToString() : "";

                // Weight Tracking
                PcosWeightContainer.IsVisible = PcosEndoHelperFunctions.HasPcosWeightRecordedThisMonth(allLogs);
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

        private void LoadEndoData(OvulationCycleLog logEntry)
        {
            try
            {
                var allLogs = _moduleLogsService.GetAllPeriodLogs();

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
                EndoWeightContainer.IsVisible = PcosEndoHelperFunctions.HasEndoWeightRecordedThisMonth(allLogs);
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
                PcosWaistHipRatio.Text = $"{ratio}";
                //PcosWaistHipRatio.TextColor = ratio > 0.85 ? Color.FromArgb("#FF0000") : Color.FromArgb("#008000");
            }
            else
            {
                PcosWaistHipRatio.Text = "--";
            }

            // Endo Ratio Calculation
            if (double.TryParse(EndoWaistMeasurement.Text, out double endoWaist) &&
                double.TryParse(EndoHipMeasurement.Text, out double endoHip) &&
                endoHip > 0)
            {
                double ratio = Math.Round(endoWaist / endoHip, 2);
                EndoWaistHipRatio.Text = $"{ratio}";
                //EndoWaistHipRatio.TextColor = ratio > 0.85 ? Color.FromArgb("#FF0000") : Color.FromArgb("#008000");
            }
            else
            {
                EndoWaistHipRatio.Text = "--";
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

        private double GetDoubleQuick(Entry value)
        {
            return Convert.ToDouble(value.Text);
        }

        private void RatioMeasurementChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PcosWaistMeasurement.Text) && !string.IsNullOrEmpty(PcosHipMeasurement.Text))
            {
                var ratio = Math.Round(GetDoubleQuick(PcosWaistMeasurement) / GetDoubleQuick(PcosHipMeasurement), 1);
                PcosWaistHipRatio.Text = "" + ratio;
            }
            else
            {
                PcosWaistHipRatio.Text = "";
            }

            if (!string.IsNullOrEmpty(EndoWaistMeasurement.Text) && !string.IsNullOrEmpty(EndoHipMeasurement.Text))
            {
                var ratio = Math.Round(GetDoubleQuick(EndoWaistMeasurement) / GetDoubleQuick(EndoHipMeasurement), 1);
                EndoWaistHipRatio.Text = "" + ratio;
            }
            else
            {
                EndoWaistHipRatio.Text = "";
            }
        }
    }
}