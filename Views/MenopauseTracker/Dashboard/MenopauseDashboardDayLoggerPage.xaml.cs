using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging.Abstractions;
using OvulaeApp.Helpers.Pages.DayLogging;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.MenopauseTracker;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers.DayLogging;
using OvulaeShared.Models.Menopause;
using System.Threading.Tasks;

namespace OvulaeApp.Views.MenopauseTracker.Dashboard
{
    [QueryProperty(nameof(EntryId), "entryId")]
    public partial class MenopauseDashboardDayLoggerPage : ContentPage
    {
        private readonly IModuleLogsService _moduleLogsServ;
        private readonly IUserLocalService _userSer;
        private readonly IMenopauseService _menopauseServ;

        private MenopauseLoggerViewModel viewModel;
        
        public bool _isNavigating { get; set; }

        public int EntryId { get; set; }

        public MenopauseDashboardDayLoggerPage(IModuleLogsService moduleLogsServ, IUserLocalService userSer, IMenopauseService menopauseServ)
        {
            InitializeComponent();

            _moduleLogsServ = moduleLogsServ;
            _userSer = userSer;
            _menopauseServ = menopauseServ;
        }

        private void EnterSymptomsManuallySwitch_Toggled(object sender, EventArgs e)
        {
            LogManualSymptom.IsVisible = ManualSymptomsSwitch.IsToggled;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                BaseTabs.SetLoaders(Spinner, AppLoader);
                SideMenu.ConfigureComponents(
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, ModuleTrackerSwitch, YesNoPopup
                );
                Header.SetLoaders(Spinner, AppLoader);

                Header.OpenSideMenuCommand = new Command(async () =>
                {
                    await SideMenu.OpenAsync();
                });

                // Initialize view model
                viewModel = new MenopauseLoggerViewModel(_moduleLogsServ);
                BindingContext = viewModel;

                await Spinner.ShowSpinnerAsync();

                if (EntryId > 0)
                {
                    await _moduleLogsServ.LoadMenopauseLogs(LocalStorageService.UserDetails.UserId);
                    viewModel.CurrentLogEntry = _moduleLogsServ.GetMenopauseLogByEntryId(EntryId);
                    viewModel.CurrentDate = viewModel.CurrentLogEntry?.LogDate ?? DateTime.Today;
                }

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open menopause logger page, error: {ex.Message}");
                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
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

        private void LoadCurrentLogData()
        {
            var TodayMenopauseLog = viewModel.CurrentLogEntry;

            if (TodayMenopauseLog == null)
            {
                TodayMenopauseLog = DefaultValueHelper.CreateWithDefaults<MenopauseLogEntry>();
                TodayMenopauseLog.PhaseName = _menopauseServ.GetMenopauseStage().GetDisplayName();
                TodayMenopauseLog.LogDate = DateTime.Now;
            }

            WeightFluctuateNormalComponent.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(WeightFluctuateComponent, false);

            var selectedMoods = TodayMenopauseLog?.Moods ?? new();
            var selectedSymptoms = TodayMenopauseLog?.Symptoms ?? new();
            var selectedSleepQuality = TodayMenopauseLog?.SleepQuality ?? "";
            var selectedHotFlashesSeverity = TodayMenopauseLog?.HotFlashesSeverity ?? "";
            var selectedNightSweatsSeverity = TodayMenopauseLog?.NightSweatsSeverity ?? "";
            var selectedLibido = TodayMenopauseLog?.Libido ?? "";
            var selectedIsOnHormoneTherapy = TodayMenopauseLog.IsOnHormoneTherapy ? "💊 Started HRT" : "";
            var hadPeriod = TodayMenopauseLog.HadBleeding ? MenopauseTrackerDayLogItems.PeriodHad[0] : "";
            
            DayLoggerHelper.PopulateMultiSelectComponent(MoodsComponent, MenopauseTrackerDayLogItems.Mood, selectedMoods);
            DayLoggerHelper.PopulateMultiSelectComponent(SymptomsComponent, MenopauseTrackerDayLogItems.Symptoms, selectedSymptoms);
            DayLoggerHelper.PopulateMultiSelectComponent(SleepQualityComponent, MenopauseTrackerDayLogItems.SleepQuality, selectedSleepQuality);
            DayLoggerHelper.PopulateMultiSelectComponent(HotflashSeverityComponent, MenopauseTrackerDayLogItems.HotFlashesSeverity, selectedHotFlashesSeverity);
            DayLoggerHelper.PopulateMultiSelectComponent(LibidoComponent, MenopauseTrackerDayLogItems.Libido, selectedLibido);
            DayLoggerHelper.PopulateMultiSelectComponent(HormoneTherapyComponent, MenopauseTrackerDayLogItems.HormoneTherapyNotesSuggestions, selectedIsOnHormoneTherapy);
            DayLoggerHelper.PopulateMultiSelectComponent(LifestyleFactorsComponent, MenopauseTrackerDayLogItems.LifestyleFactors, TodayMenopauseLog.LifestyleFactors);
            DayLoggerHelper.PopulateMultiSelectComponent(PeriodHadComponent, MenopauseTrackerDayLogItems.PeriodHad, hadPeriod);
            DayLoggerHelper.PopulateMultiSelectComponent(BleedingTypesComponent, MenopauseTrackerDayLogItems.BleedingTypes, TodayMenopauseLog.BleedingType);
            DayLoggerHelper.PopulateMultiSelectComponent(BleedingColorsComponent, MenopauseTrackerDayLogItems.BleedingColors, TodayMenopauseLog.BleedingColor);
            DayLoggerHelper.PopulateMultiSelectComponent(BleedingPatternsComponent, MenopauseTrackerDayLogItems.BleedingPatterns, TodayMenopauseLog.BleedingPattern);
            DayLoggerHelper.PopulateMultiSelectComponent(HadPelvicPainComponent, MenopauseTrackerDayLogItems.HadPelvicPain, TodayMenopauseLog.HadPelvicPain ? MenopauseTrackerDayLogItems.HadPelvicPain[0] : "");
            DayLoggerHelper.PopulateMultiSelectComponent(PelvicPainSeverityComponent, MenopauseTrackerDayLogItems.PelvicPainSeverity, TodayMenopauseLog.PainSeverity);

            DayLoggerHelper.PopulateMultiSelectComponent(CoughWeeComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(SneezeWeeComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(LaughWeeComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(BladderPainComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(UTIComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(HairLossComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(WeightGainComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(WeightFluctuateComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(WeightFluctuateNormalComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(BloodPressureDailyComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(BloodPressureMedicationComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(BlemishesComponent, DayLoggerHelper.YesNoOption, "");
            DayLoggerHelper.PopulateMultiSelectComponent(MedAndSupplementComponent, MedicationSupplementDayLogItems.MenopauseTrackerMedications, TodayMenopauseLog.SupplementsAndMedication);

            var bp = !string.IsNullOrEmpty(TodayMenopauseLog.BloodPressureReadings) ? TodayMenopauseLog.BloodPressureReadings.Split('/') : new string[] { "80", "50" };
            BpSystolicValue.Value = Convert.ToInt32(bp[0]);
            BpDiastolicValue.Value = Convert.ToInt32(bp[1]);

            ReflectionText.Text = TodayMenopauseLog.Notes;

            PeriodOptions.IsVisible = PeriodHadComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.PeriodHad[0]);
            PelvicPainSeverityComponent.IsVisible = HadPelvicPainComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.HadPelvicPain[0]);

            CoughWeeNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.CoughWeeNotes);
            SneezeWeeNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.SneezeWeeNotes);
            LaughWeeNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.LaughWeeNotes);
            BladderPainNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.BladderPainNotes);
            HairLossNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.HairLossNotes);
            WeightGainNotes.Text = DefaultValueHelper.GetStringValueOrDefault(TodayMenopauseLog.WeightGainNotes);
            MedicationAndSupplementsNotes.Text = TodayMenopauseLog.MedicationNotes;

            UrinationRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(TodayMenopauseLog.UrinationRating);
            BreastTendernessRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(TodayMenopauseLog.BreastTendernessRating);
            SkinDrynessRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(TodayMenopauseLog.SkinDrynessRating);

            // Update visibility of rating containers based on selections
            UpdateRatingContainersVisibility();
        }

        private void PeriodHadComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            PeriodOptions.IsVisible = PeriodHadComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.PeriodHad[0]);
        }

        private void HadPelvicPainComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            PelvicPainSeverityComponent.IsVisible = HadPelvicPainComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.HadPelvicPain[0]);
        }

        private void UpdateLogEntryFromUI() 
        {
            var TodayMenopauseLog = viewModel.CurrentLogEntry;

            var selectedMoods = MoodsComponent.SelectedItems.ToList();
            var selectedSymptoms = SymptomsComponent.SelectedItems.ToList();
            var selectedSleepQuality = SleepQualityComponent.SelectedItems.FirstOrDefault();
            var selectedHotFlashesSeverity = SleepQualityComponent.SelectedItems.FirstOrDefault();
            var selectedNightSweatsSeverity = SleepQualityComponent.SelectedItems.FirstOrDefault();
            var selectedLibido = SleepQualityComponent.SelectedItems.FirstOrDefault();
            var selectedIsOnHormoneTherapy = SleepQualityComponent.SelectedItems.FirstOrDefault();
            var notes = ReflectionText?.Text ?? "";
            var manualSymptom = LogManualSymptomText?.Text ?? "";

            TodayMenopauseLog.LogDate = DateTime.Now;
            TodayMenopauseLog.PhaseName = _menopauseServ.GetMenopauseStage().GetDisplayName();
            TodayMenopauseLog.Moods = selectedMoods;
            TodayMenopauseLog.Symptoms = selectedSymptoms;
            TodayMenopauseLog.NightSweatsSeverity = selectedNightSweatsSeverity;
            TodayMenopauseLog.HotFlashesSeverity = selectedHotFlashesSeverity;
            TodayMenopauseLog.Libido = selectedLibido;
            TodayMenopauseLog.Notes = notes;
            TodayMenopauseLog.IsOnHormoneTherapy = selectedIsOnHormoneTherapy == "💊 Started HRT";
            TodayMenopauseLog.LifestyleFactors = LifestyleFactorsComponent.SelectedItems.ToList();
            TodayMenopauseLog.HadBleeding = PeriodHadComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.PeriodHad[0]);
            TodayMenopauseLog.BleedingColor = BleedingColorsComponent.SelectedItems.FirstOrDefault();
            TodayMenopauseLog.BleedingPattern = BleedingPatternsComponent.SelectedItems.FirstOrDefault();
            TodayMenopauseLog.HadPelvicPain = HadPelvicPainComponent.SelectedItems.Contains(MenopauseTrackerDayLogItems.HadPelvicPain[0]);
            TodayMenopauseLog.PainSeverity = PelvicPainSeverityComponent.SelectedItems.FirstOrDefault();

            TodayMenopauseLog.CoughWeeYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(CoughWeeComponent, TodayMenopauseLog.CoughWeeYesNo);
            TodayMenopauseLog.SneezeWeeYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(SneezeWeeComponent, TodayMenopauseLog.SneezeWeeYesNo);
            TodayMenopauseLog.LaughWeeYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(LaughWeeComponent, TodayMenopauseLog.LaughWeeYesNo);
            TodayMenopauseLog.BladderPainYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(BladderPainComponent, TodayMenopauseLog.BladderPainYesNo);
            TodayMenopauseLog.UTIOften = DayLoggerHelper.GetBooleanFromYesNoSelection(UTIComponent, TodayMenopauseLog.UTIOften);
            TodayMenopauseLog.HairLossYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(HairLossComponent, TodayMenopauseLog.HairLossYesNo);
            TodayMenopauseLog.WeightGainYesNo = DayLoggerHelper.GetBooleanFromYesNoSelection(WeightGainComponent, TodayMenopauseLog.WeightGainYesNo);
            TodayMenopauseLog.WeightFluctuate = DayLoggerHelper.GetBooleanFromYesNoSelection(WeightFluctuateComponent, TodayMenopauseLog.WeightFluctuate);
            TodayMenopauseLog.WeightFluctuateNormal = DayLoggerHelper.GetBooleanFromYesNoSelection(WeightFluctuateNormalComponent, TodayMenopauseLog.WeightFluctuateNormal);
            TodayMenopauseLog.BloodPressureDaily = DayLoggerHelper.GetBooleanFromYesNoSelection(BloodPressureDailyComponent, TodayMenopauseLog.BloodPressureDaily);
            TodayMenopauseLog.BlemishShowing = DayLoggerHelper.GetBooleanFromYesNoSelection(BlemishesComponent, TodayMenopauseLog.BlemishShowing);

            TodayMenopauseLog.CoughWeeNotes = CoughWeeNotes.Text;
            TodayMenopauseLog.SneezeWeeNotes = SneezeWeeNotes.Text;
            TodayMenopauseLog.LaughWeeNotes = LaughWeeNotes.Text;
            TodayMenopauseLog.BladderPainNotes = BladderPainNotes.Text;
            TodayMenopauseLog.HairLossNotes = HairLossNotes.Text;
            TodayMenopauseLog.WeightGainNotes = WeightGainNotes.Text;
            TodayMenopauseLog.SupplementsAndMedication = MedAndSupplementComponent.SelectedItems.ToList();
            TodayMenopauseLog.MedicationNotes = MedicationAndSupplementsNotes.Text;

            TodayMenopauseLog.UrinationRating = UrinationRating.SelectedRating;
            TodayMenopauseLog.BreastTendernessRating = BreastTendernessRating.SelectedRating;
            TodayMenopauseLog.SkinDrynessRating = SkinDrynessRating.SelectedRating;
        }

        private async void SaveTodaysLogs_Clicked(object sender, EventArgs e)
        {
            try
            {
                await AppLoader.ShowAsync("Saving your logs...");

                // Update the viewModel's CurrentLogEntry from UI
                UpdateLogEntryFromUI();

                // Save through viewModel
                var success = await viewModel.SaveCurrentLog();

                if (success)
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Saved", "Your today's experience was saved😊."));
                }
                else
                {
                    await AppLoader.HideAsync();
                    await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed to save", "We couldn't save your logs 🙁."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to capture today's Logs: {ex.Message}");
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "We couldn't save your logs 🙁."));
            }
            finally
            {
                await AppLoader.HideAsync();
            }
        }

        private void WeightFluctuateComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            WeightFluctuateNormalComponent.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(WeightFluctuateComponent);
        }

        private void UpdateRatingContainersVisibility()
        {
            CoughWeeContainerRating.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(CoughWeeComponent);
            SneezeWeeContainerRating.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(SneezeWeeComponent);
            LaughWeeContainerRating.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(LaughWeeComponent);
            BladderPainContainerRating.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(BladderPainComponent);
            HairLossNotesContainer.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(HairLossComponent);
            WeightGainContainerRating.IsVisible = DayLoggerHelper.GetBooleanFromYesNoSelection2(WeightGainComponent);
        }

        private void SelectComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            UpdateRatingContainersVisibility();
        }

        private void BloodPressureDailyComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            BloodPressureReadingsContainer.IsVisible = BloodPressureDailyComponent.SelectedItems.Contains(DayLoggerHelper.YesNoOption[0]);
        }

        private void BpValue_ValueChanged(object sender, int e)
        {
            BpText.Text = $"*This reads {BpSystolicValue.Value}/{BpDiastolicValue.Value} mmHg";
        }
    }
}
