using System;
using System.Collections.Generic;
using System.Windows.Input;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.Enums.User;
using OvulaeShared.Services.Module.CycleServices;
using OvulaeShared.ViewModel.Education;
using OvulaeShared.ViewModel.Module;

namespace OvulaeApp.ViewModels.Shared
{
    public class PeriodOvulationTrackViewModel : BaseViewModel
    {
        private readonly IEducationService _eduServ;
        private readonly SpinnerLoader spinner;

        private ModulePhaseDataViewModel _moduleData;
        public ModulePhaseDataViewModel ModuleData
        {
            get => _moduleData;
            set
            {
                _moduleData = value;
                OnPropertyChanged();
            }
        }

        public List<EducationCoverGroup> EducationGroupsCovers { get; set; }

        public string CurrentPhaseDayTitle { get; set; }
        public string CurrentPhaseNameTitle { get; set; }
        public string CurrentPhaseDateRangeTitle { get; set; }

        private bool _resetPhaseButtonVisible;
        public bool ResetPhaseButtonVisible
        {
            get => _resetPhaseButtonVisible;
            set
            {
                _resetPhaseButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToPreviousPhaseCommand { get; }
        public ICommand GoToNextPhaseCommand { get; }
        public ICommand GoToCurrentPhaseCommand { get; }

        private DateTime LMP;
        private OvulationPhase _ovulationPhase;
        private OvulationPhase originalPhase;
        private int phaseCycleOffset;

        public OvulationPhase OvulationPhase
        {
            get => _ovulationPhase;
            set
            {
                _ovulationPhase = value;
                ModuleData = Data.FirstOrDefault(x => x.ModulePhase == _ovulationPhase);
                OnPropertyChanged();
            }
        }

        private bool _showPeriodTrackerData;
        
        public bool ShowPeriodTrackerData 
        {
            get => _showPeriodTrackerData;
            set
            {
                _showPeriodTrackerData = value;
                OnPropertyChanged();
            }
        }

        private bool _showPeriodTrackerNoData;

        public bool ShowPeriodTrackerNoData
        {
            get => _showPeriodTrackerNoData;
            set
            {
                _showPeriodTrackerNoData = value;
                OnPropertyChanged();
            }
        }

        public bool IsMainUser { get; set; }

        private List<ModulePhaseDataViewModel> Data;
        private DateTime CurrentPhaseStartDate;
        private DateTime CurrentPhaseEndDate;

        private DateTime currentPhaseDate;

        private ModuleType moduleType;

        private readonly ICycleService _cylceServ;

        public PeriodOvulationTrackViewModel(ICycleService cylceServ, List<ModulePhaseDataViewModel> data, IEducationService eduServ, SpinnerLoader spinner, ModuleType moduleType)
        {
            _cylceServ = cylceServ;
            _eduServ = eduServ;

            this.spinner = spinner;
            this.moduleType = moduleType;

            Data = data;

            GoToPreviousPhaseCommand = new Command(GoToPreviousPhase);
            GoToNextPhaseCommand = new Command(GoToNextPhase);
            GoToCurrentPhaseCommand = new Command(GoToCurrentPhase);

            ShowPeriodTrackerData = LocalStorageService.PeriodTrackerSet;
            ShowPeriodTrackerNoData = !LocalStorageService.PeriodTrackerSet;

            LoadPageData();
        }

        public void ShowPeriodData()
        {
            ShowPeriodTrackerData = true;
            ShowPeriodTrackerNoData = false;

            LoadPageData();
        }

        private void LoadPageData()
        {
            try
            {
                EducationGroupsCovers = _eduServ.GetModuleEducationGroupsCoversByType(moduleType, EducationCategoryType.Recommended) ?? new List<EducationCoverGroup>();
                IsMainUser = LocalStorageService.UserDetails.UserRole == UserRoleType.Client;
                if (ShowPeriodTrackerData) 
                {
                    _cylceServ.LoadCycleData(LocalStorageService.UserCycleProfile);

                    phaseCycleOffset = 0;

                    OvulationPhase = _cylceServ.GetCurrentPhase();
                    originalPhase = OvulationPhase;

                    (CurrentPhaseStartDate, CurrentPhaseEndDate) = _cylceServ.CalculatePhaseDateRange(OvulationPhase, DateTime.Today, phaseCycleOffset, true);

                    currentPhaseDate = CurrentPhaseStartDate;

                    ModuleData = Data.FirstOrDefault(x => x.ModulePhase == OvulationPhase);
                   
                    UpdatePhaseTitles(OvulationPhase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ovulation dashboard: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Failed to load dashboard.", "OK");
            }
        }

        private void UpdatePhaseTitles(OvulationPhase phase)
        {
            var currentCycleDay = _cylceServ.GetCurrentCycleDay();

            var referenceDate = DateTime.Now;
            int totalDays = _cylceServ.GetPhaseLength(phase);
            int phaseDay = _cylceServ.GetPhaseDayWithinPhase(phase, referenceDate);
            int cycleLength = _cylceServ.GetCycleLength();

            CurrentPhaseDayTitle = $"Day {phaseDay} of {totalDays}: {phase} Phase";
            CurrentPhaseNameTitle = $"Cycle Day {currentCycleDay} of {cycleLength}";

            CurrentPhaseDateRangeTitle = $"🗓️ {CurrentPhaseStartDate:dd MMM} – {CurrentPhaseEndDate:dd MMM yyyy}";

            OnPropertyChanged(nameof(CurrentPhaseDayTitle));
            OnPropertyChanged(nameof(CurrentPhaseNameTitle));
            OnPropertyChanged(nameof(CurrentPhaseDateRangeTitle));
        }

        public void GoToPreviousPhase()
        {
            if (OvulationPhase != OvulationPhase.Menstrual)
            {
                OvulationPhase--;
            }
            else
            {
                phaseCycleOffset--;
                OvulationPhase = OvulationPhase.Luteal;
            }

            CurrentPhaseEndDate = CurrentPhaseStartDate.AddDays(-1);
            CurrentPhaseStartDate = CurrentPhaseEndDate.AddDays(-_cylceServ.GetPhaseLength(OvulationPhase) + 1);

            ResetPhaseButtonVisible = CurrentPhaseStartDate != currentPhaseDate;

            UpdatePhaseTitles(OvulationPhase);
        }

        public void GoToNextPhase()
        {
            if (OvulationPhase != OvulationPhase.Luteal)
            {
                OvulationPhase++;
            }
            else
            {
                phaseCycleOffset++;
                OvulationPhase = OvulationPhase.Menstrual;
            }

            CurrentPhaseStartDate = CurrentPhaseEndDate.AddDays(1);
            CurrentPhaseEndDate = CurrentPhaseStartDate.AddDays(_cylceServ.GetPhaseLength(OvulationPhase) - 1);

            ResetPhaseButtonVisible = CurrentPhaseStartDate != currentPhaseDate;

            UpdatePhaseTitles(OvulationPhase);
        }

        public void GoToCurrentPhase()
        {
            phaseCycleOffset = 0;
            ResetPhaseButtonVisible = false;

            LoadPageData();
        }
    }
}
