using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.ViewModels.Module;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.Enums.User;
using OvulaeShared.ViewModel.Education;
using OvulaeShared.ViewModel.Module;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseTrackViewModel : BaseViewModel
    {
        private readonly IEducationService _eduServ;
        private readonly SpinnerLoader spinner;
        private readonly BrandedLoader appLoader;

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

        public ICommand GoToPreviousPhaseCommand { get; }
        public ICommand GoToNextPhaseCommand { get; }
        public ICommand GoToCurrentPhaseCommand { get; }

        public List<EducationCoverGroup> EducationGroupsCovers { get; set; }

        public string CurrentPhaseNameTitle { get; set; }
        public string CurrentPhaseSubtitle { get; set; }
        public string CurrentPhaseDurationTitle { get; set; }

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

        private DateTime LMP;

        public bool IsMainUser { get; set; }

        private ModulePhaseDataViewModel Data;
        
        public MenopauseStage MenopauseStage;

        private readonly IMenopauseService _menopauseServ;

        public MenopauseStage currentPhase { get; set; }

        public MenopauseTrackViewModel(IMenopauseService menopauseServ, IEducationService eduServ, SpinnerLoader spinner)
        {
            _menopauseServ = menopauseServ;
            _eduServ = eduServ;

            this.spinner = spinner;
            
            GoToPreviousPhaseCommand = new Command(GoToPreviousPhase);
            GoToNextPhaseCommand = new Command(GoToNextPhase);
            GoToCurrentPhaseCommand = new Command(GoToCurrentPhase);

            LoadPageData();
        }

        private void LoadPageData()
        {
            try
            {
                EducationGroupsCovers = _eduServ.GetModuleEducationGroupsCoversByType(ModuleType.MenopauseTracker, EducationCategoryType.Recommended) ?? new List<EducationCoverGroup>();
                IsMainUser = LocalStorageService.UserDetails.UserRole == UserRoleType.Client;
                
                LMP = LocalStorageService.UserCycleProfile.LastPeriodDate.Value;

                MenopauseStage = _menopauseServ.GetMenopauseStage();
                currentPhase = MenopauseStage;

                UpdateMenopausePhaseTitles(MenopauseStage);
            }
            catch (Exception ex)
            {
                Shell.Current.CurrentPage.ShowPopup(new BrandedAlertPopup("Error", "Failed to load dashboard.", "OK"));
            }
        }

        private void UpdateMenopausePhaseTitles(MenopauseStage stage)
        {
            ModuleData = _menopauseServ.GetCurrentPhaseDataForStage(stage);

            CurrentPhaseNameTitle = $"{stage.GetDisplayName()} Stage";
            ResetPhaseButtonVisible = stage != currentPhase;

            var monthsSince = (DateTime.Now.Year - LMP.Year) * 12 +
                                  (DateTime.Now.Month - LMP.Month);
            CurrentPhaseSubtitle = $"Last period: {LMP:dd MMM yyyy} • {monthsSince} months ago";

            CurrentPhaseDurationTitle = $"Tracking for {GetTrackingDuration()}";

            OnPropertyChanged(nameof(CurrentPhaseNameTitle));
            OnPropertyChanged(nameof(CurrentPhaseSubtitle));
            OnPropertyChanged(nameof(CurrentPhaseDurationTitle));
        }

        private string GetTrackingDuration()
        {
            DateTime? startDate = LocalStorageService.UserCycleProfile.TrackingStartDate;
            if (startDate == null)
                return "an unknown time";

            var timespan = DateTime.Now - startDate.Value;
            int months = (int)(timespan.Days / 30.4);
            return months > 1 ? $"{months} months" : "less than a month";
        }

        public void GoToPreviousPhase()
        {
            if (MenopauseStage == MenopauseStage.Premenopause)
            {
                MenopauseStage = MenopauseStage.Postmenopause;
            }
            else
            {
                MenopauseStage--;
            }
            UpdateMenopausePhaseTitles(MenopauseStage);
        }

        public void GoToNextPhase()
        {
            if (MenopauseStage == MenopauseStage.Postmenopause)
            {
                MenopauseStage = MenopauseStage.Premenopause;
            }
            else
            {
                MenopauseStage++;
            }
            UpdateMenopausePhaseTitles(MenopauseStage);
        }

        public void GoToCurrentPhase()
        {
            LoadPageData();
        }
    }
}
