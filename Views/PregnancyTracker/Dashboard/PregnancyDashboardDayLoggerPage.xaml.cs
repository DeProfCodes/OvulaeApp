using System;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Pages.DayLogging;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Modals;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers.DayLogging;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Pregnancy;
using static OvulaeShared.Helpers.API.OvulaeApiEndPoints;

namespace OvulaeApp.Views.PregnancyTracker.Dashboard
{
    [QueryProperty(nameof(EntryId), "entryId")]
    public partial class PregnancyDashboardDayLoggerPage : ContentPage
    {
        public int EntryId { get; set; }

        private readonly IModuleLogsService _moduleLogsServ;
        private readonly IUserLocalService _userSer;
        private PregnancyLoggerViewModel viewModel;

        public bool _isNavigating { get; set; }

        public PregnancyDashboardDayLoggerPage(IPregnancyService pregServ, IUserLocalService userSer, IModuleLogsService moduleLogsServ)
        {
            InitializeComponent();
            _moduleLogsServ = moduleLogsServ;
            _userSer = userSer;
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
                    Spinner, AppLoader, PregnancyTrackerOnBoard, PeriodTrackerOnBoard, PregnancyComplete, ModuleTrackerSwitch,
                    YesNoModal, MenopauseTrackerOnBoard
                );

                Header.SetLoaders(Spinner, AppLoader);
                Header.OpenSideMenuCommand = new Command(async () => await SideMenu.OpenAsync());

                // Initialize view model
                viewModel = new PregnancyLoggerViewModel(_moduleLogsServ);
                BindingContext = viewModel;

                await Spinner.ShowSpinnerAsync();

                if (EntryId > 0)
                {
                    await _moduleLogsServ.LoadPregnancyLogs(LocalStorageService.UserDetails.UserId);
                    viewModel.CurrentLogEntry = _moduleLogsServ.GetPregnancyLogByEntryId(EntryId);
                    viewModel.CurrentDate = viewModel.CurrentLogEntry?.LogDate ?? DateTime.Today; 
                }

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open pregnancy logger page, error: {ex.Message}");

                UpdateDayNavigationButtons();
                LoadCurrentLogData();

                await Spinner.HideSpinnerAsync();
            }
        }

        private void LoadCurrentLogData()
        {
            try
            {
                var logEntry = viewModel.CurrentLogEntry ?? new();

                // Populate all components with current log data
                DayLoggerHelper.PopulateMultiSelectComponent(MoodsComponent, PregnancyDayLogItems.Moods, logEntry.Moods);
                DayLoggerHelper.PopulateMultiSelectComponent(SymptomsComponent, PregnancyDayLogItems.Symptoms, logEntry.Symptoms);
                DayLoggerHelper.PopulateMultiSelectComponent(BabyMovementsComponent, PregnancyDayLogItems.BabyMovements, logEntry.BabyMovements);
                DayLoggerHelper.PopulateMultiSelectComponent(ActivitiesComponent, PregnancyDayLogItems.PregnancyActivities, logEntry.Activities);
                DayLoggerHelper.PopulateMultiSelectComponent(VitaminComponent, PregnancyDayLogItems.VitaminSupplements, logEntry.Vitamins);
                DayLoggerHelper.PopulateMultiSelectComponent(CheckupsComponent, PregnancyDayLogItems.Checkups, logEntry.Checkups);
                DayLoggerHelper.PopulateMultiSelectComponent(SleepComponent, PregnancyDayLogItems.SleepQuality, logEntry.SleepQuality);
                DayLoggerHelper.PopulateMultiSelectComponent(DiscomfortsComponent, PregnancyDayLogItems.Discomforts, logEntry.DiscomfortDescription);
                DayLoggerHelper.PopulateMultiSelectComponent(BleedingComponent, PregnancyDayLogItems.BleedingOrLeakage, logEntry.BleedingDetails);
                DayLoggerHelper.PopulateMultiSelectComponent(StressComponent, PregnancyDayLogItems.StressLevels, logEntry.StressLevel);
                DayLoggerHelper.PopulateMultiSelectComponent(CravingsComponent, PregnancyDayLogItems.Cravings, logEntry.Cravings);
                DayLoggerHelper.PopulateMultiSelectComponent(AppetiteComponent, PregnancyDayLogItems.AppetiteLevels, logEntry.AppetiteLevel);
                DayLoggerHelper.PopulateMultiSelectComponent(EnergyComponent, PregnancyDayLogItems.EnergyLevels, logEntry.EnergyLevel);
                DayLoggerHelper.PopulateMultiSelectComponent(WaterIntakeComponent, PregnancyDayLogItems.WaterIntakeCups, logEntry.WaterIntake);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsExistComponent, DayLoggerHelper.YesNoOption, "");
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsRegularityComponent, CycleTrackerDayLogItems.PeriodBowelMovementIrregularityOptions, logEntry.BowelMovementsRegularity);
                DayLoggerHelper.PopulateMultiSelectComponent(BowelMovementsFrequencyComponent, CycleTrackerDayLogItems.PeriodBowelMovementFrequencyOptions, logEntry.BowelMovementsFrequency);
                DayLoggerHelper.PopulateMultiSelectComponent(UTIComponent, DayLoggerHelper.YesNoOption, "");
                DayLoggerHelper.PopulateMultiSelectComponent(BloodPressureDailyComponent, DayLoggerHelper.YesNoOption, "");
                DayLoggerHelper.PopulateMultiSelectComponent(BloodPressureMedicationComponent, DayLoggerHelper.YesNoOption, "");
                DayLoggerHelper.PopulateMultiSelectComponent(BrestFeelingComponent, PregnancyDayLogItems.BrestFeeeling, logEntry.BrestFeeling);
                DayLoggerHelper.PopulateMultiSelectComponent(NightUrinationComponent, PregnancyDayLogItems.NightUrination, logEntry.NighlyUrination);
                DayLoggerHelper.PopulateMultiSelectComponent(MedAndSupplementComponent, MedicationSupplementDayLogItems.PregnancyTrackerMedications, logEntry.SupplementsAndMedication);

                // Set ratings
                MoodRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.MoodsRating);
                SymptomsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.SymptomsRating);
                StressRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.StressRating);
                CravingsRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.CravingsRating);
                AppetiteRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.AppetiteRating);
                EnergyRating.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.EnergyRating);
                BabyMovementsRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.BabyMovementsRating);
                ActivitiesRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.ActivitiesRating);
                VitaminsRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.VitaminsRating);
                SleepRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.SleepRating);
                DiscomfortRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.DiscomfortRating);
                BleedingRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.BleedingRating);
                BrestFeelingRatings.SelectedRating = DefaultValueHelper.GetIntValueOrDefault(logEntry.BrestFeelingRating);

                // Set notes
                MoodNotes.Text = logEntry.MoodsNotes;
                SymptomsNotes.Text = logEntry.SymptomsNotes;
                StressNotes.Text = logEntry.StressNotes;
                CravingsNotes.Text = logEntry.CravingsNotes;
                AppetiteNotes.Text = logEntry.AppetiteNotes;
                EnergyNotes.Text = logEntry.EnergyNotes;
                BabyMovementsNotes.Text = logEntry.BabyMovementsNotes;
                ActivitiesNotes.Text = logEntry.ActivitiesNotes;
                VitaminsNotes.Text = logEntry.VitaminsNotes;
                SleepNotes.Text = logEntry.SleepNotes;
                DiscomfortNotes.Text = logEntry.DiscomfortNotes;
                BleedingNotes.Text = logEntry.BleedingNotes;
                BrestFeelingNotes.Text = logEntry.BrestFeelingNotes;
                MedicationAndSupplementsNotes.Text = logEntry.MedicationNotes;

                ReflectionText.Text = logEntry.Reflection;

                // Handle blood pressure
                var bp = logEntry.BloodPressureReadings != null ? logEntry.BloodPressureReadings.Split('/') : new string[] { "80", "50" };
                BpSystolicValue.Value = Convert.ToInt32(bp.Length > 0 ? bp[0] : "80");
                BpDiastolicValue.Value = Convert.ToInt32(bp.Length > 1 ? bp[1] : "50");

                DoctorNotesContainer.IsVisible = !string.IsNullOrEmpty(logEntry.DoctorsNotes);
                DoctorsNotes.Text = logEntry.DoctorsNotes;

                // Update visibility of rating containers based on selections
                UpdateRatingContainersVisibility();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load log data: {ex.Message}");
            }
        }

        private void UpdateDayNavigationButtons()
        {
            try
            {
                // Hide next day button if we're already at today
                NextDayBtn.IsVisible = viewModel.CurrentDate < DateTime.Today;

                // Always show previous day button (unless you want to limit how far back they can go)
                PrevDayBtn.IsVisible = true;
            }
            catch
            {
                
            }
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

        private async void SaveTodaysLogs_Clicked(object sender, EventArgs e)
        {
            if (_isNavigating) return;

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
                Console.WriteLine($"Failed to capture today's Logs: {ex.Message}");
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "We couldn't save your logs 🙁."));
            }
            finally
            {
                await AppLoader.HideAsync();
                _isNavigating = false;
            }
        }

        private void UpdateLogEntryFromUI()
        {
            var logEntry = viewModel.CurrentLogEntry;

            // Update all properties from UI components
            logEntry.Moods = MoodsComponent.SelectedItems.ToList();
            logEntry.Symptoms = SymptomsComponent.SelectedItems.ToList();

            if (ManualSymptomsSwitch.IsToggled && !string.IsNullOrEmpty(LogManualSymptomText.Text))
            {
                logEntry.Symptoms.Add(LogManualSymptomText.Text);
            }

            logEntry.BabyMovements = BabyMovementsComponent.SelectedItems.ToList();
            logEntry.Activities = ActivitiesComponent.SelectedItems.ToList();
            logEntry.Vitamins = VitaminComponent.SelectedItems.ToList();
            logEntry.Checkups = CheckupsComponent.SelectedItems.ToList();
            logEntry.SleepQuality = SleepComponent.SelectedItems.FirstOrDefault();
            logEntry.DiscomfortDescription = DiscomfortsComponent.SelectedItems.ToList();
            logEntry.BleedingDetails = BleedingComponent.SelectedItems.FirstOrDefault();
            logEntry.StressLevel = StressComponent.SelectedItems.FirstOrDefault();
            logEntry.Cravings = CravingsComponent.SelectedItems.ToList();
            logEntry.AppetiteLevel = AppetiteComponent.SelectedItems.FirstOrDefault();
            logEntry.EnergyLevel = EnergyComponent.SelectedItems.FirstOrDefault();
            logEntry.WaterIntake = WaterIntakeComponent.SelectedItems.FirstOrDefault();
            logEntry.UTIOften = DayLoggerHelper.GetBooleanFromYesNoSelection(UTIComponent, logEntry.UTIOften);
            logEntry.BloodPressureDaily = DayLoggerHelper.GetBooleanFromYesNoSelection(BloodPressureDailyComponent, logEntry.BloodPressureDaily);
            logEntry.BloodPressureReadings = logEntry.BloodPressureDaily.Value ? $"{BpSystolicValue.Value}/{BpDiastolicValue.Value}" : logEntry.BloodPressureReadings;
            logEntry.OnBloodPressureMedication = DayLoggerHelper.GetBooleanFromYesNoSelection(BloodPressureMedicationComponent, logEntry.OnBloodPressureMedication);

            logEntry.Reflection = ReflectionText.Text;

            logEntry.BowelMovementsRegularity = BowelMovementsRegularityComponent.SelectedItems.FirstOrDefault();
            logEntry.BowelMovementsFrequency = BowelMovementsFrequencyComponent.SelectedItems.FirstOrDefault();
            logEntry.NighlyUrination = NightUrinationComponent.SelectedItems.FirstOrDefault();
            logEntry.BrestFeeling = BrestFeelingComponent.SelectedItems.FirstOrDefault();

            // Update ratings
            logEntry.MoodsRating = MoodsComponent.SelectedItems.Count() > 0 ? MoodRating.SelectedRating : null;
            logEntry.SymptomsRating = SymptomsComponent.SelectedItems.Count() > 0 ? SymptomsRating.SelectedRating : null;
            logEntry.StressRating = StressComponent.SelectedItems.Count() > 0 ? StressRatings.SelectedRating : null;
            logEntry.CravingsRating = CravingsComponent.SelectedItems.Count() > 0 ? CravingsRating.SelectedRating : null;
            logEntry.AppetiteRating = AppetiteComponent.SelectedItems.Count() > 0 ? AppetiteRating.SelectedRating : null;
            logEntry.EnergyRating = EnergyComponent.SelectedItems.Count() > 0 ? EnergyRating.SelectedRating : null;
            logEntry.BabyMovementsRating = BabyMovementsComponent.SelectedItems.Count() > 0 ? BabyMovementsRatings.SelectedRating : null;
            logEntry.ActivitiesRating = ActivitiesComponent.SelectedItems.Count() > 0 ? ActivitiesRatings.SelectedRating : null;
            logEntry.VitaminsRating = VitaminComponent.SelectedItems.Count() > 0 ? VitaminsRatings.SelectedRating : null;
            logEntry.SleepRating = SleepComponent.SelectedItems.Count() > 0 ? SleepRatings.SelectedRating : null;
            logEntry.DiscomfortRating = DiscomfortsComponent.SelectedItems.Count() > 0 ? DiscomfortRatings.SelectedRating : null;
            logEntry.BleedingRating = BleedingComponent.SelectedItems.Count() > 0 ? BleedingRatings.SelectedRating : null;
            logEntry.BrestFeelingRating = BrestFeelingComponent.SelectedItems.Count() > 0 ? BrestFeelingRatings.SelectedRating : null;

            // Update notes
            logEntry.MoodsNotes = MoodNotes.Text;
            logEntry.SymptomsNotes = SymptomsNotes.Text;
            logEntry.StressNotes = StressNotes.Text;
            logEntry.CravingsNotes = CravingsNotes.Text;
            logEntry.AppetiteNotes = AppetiteNotes.Text;
            logEntry.EnergyNotes = EnergyNotes.Text;
            logEntry.BabyMovementsNotes = BabyMovementsNotes.Text;
            logEntry.ActivitiesNotes = ActivitiesNotes.Text;
            logEntry.VitaminsNotes = VitaminsNotes.Text;
            logEntry.SleepNotes = SleepNotes.Text;
            logEntry.DiscomfortNotes = DiscomfortNotes.Text;
            logEntry.BleedingNotes = BleedingNotes.Text;
            logEntry.BrestFeelingNotes = BrestFeelingNotes.Text;
            logEntry.MedicationNotes = MedicationAndSupplementsNotes.Text;

            logEntry.SupplementsAndMedication = MedAndSupplementComponent.SelectedItems.ToList();

            DefaultValueHelper.SetDefaults(logEntry);
        }

        private void UpdateRatingContainersVisibility()
        {
            MoodRatingsContainer.IsVisible = MoodsComponent.SelectedItems.Count() > 0;
            SymptomsContainerRating.IsVisible = SymptomsComponent.SelectedItems.Count() > 0;
            BabyMovementsRatingsContainer.IsVisible = BabyMovementsComponent.SelectedItems.Count() > 0;
            ActivitiesRatingsContainer.IsVisible = ActivitiesComponent.SelectedItems.Count() > 0;
            VitaminsRatingsContainer.IsVisible = VitaminComponent.SelectedItems.Count() > 0;
            SleepRatingsContainer.IsVisible = SleepComponent.SelectedItems.Count() > 0;
            DiscomfortRatingsContainer.IsVisible = DiscomfortsComponent.SelectedItems.Count() > 0;
            BleedingRatingsContainer.IsVisible = BleedingComponent.SelectedItems.Count() > 0;
            StressRatingsContainer.IsVisible = StressComponent.SelectedItems.Count() > 0;
            CravingsContainerRating.IsVisible = CravingsComponent.SelectedItems.Count() > 0;
            AppetiteContainerRating.IsVisible = AppetiteComponent.SelectedItems.Count() > 0;
            EnergyContainerRating.IsVisible = EnergyComponent.SelectedItems.Count() > 0;
            BrestFeelingRatingsContainer.IsVisible = BrestFeelingComponent.SelectedItems.Count() > 0;
        }

        private async void GivenBirthTapped(object sender, TappedEventArgs e)
        {
            try
            {
                var pregnancyRecorded = await SideMenu.RecordCompletePregnancy();
                if (pregnancyRecorded)
                {
                    var switchStatus = await SideMenu.HandlePeriodOvulationTrackerSwitch(ModuleType.PeriodTracker);
                    if (!switchStatus)
                    {
                        await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Failed to switch", "Something went wrong, failed to switch to Period Tracker at this time."));
                    }
                }
            }
            catch (Exception ex)
            {
                await AppLoader.HideAsync();
                await Shell.Current.CurrentPage.ShowPopupAsync(new BrandedAlertPopup("Error", "Something went wrong, Please try again later."));
            }
        }

        private void BowelMovementsExistComponent_SelectionChanged(object sender, IEnumerable<string> e)
        {
            var hasBowelMovements = BowelMovementsExistComponent.SelectedItems.Contains(DayLoggerHelper.YesNoOption[0]);

            BowelMovementsRegularityComponent.IsVisible = hasBowelMovements;
            BowelMovementsFrequencyComponent.IsVisible = hasBowelMovements;
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