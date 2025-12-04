using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.ModuleHelpers;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.ViewModel.Diet;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyDashboardViewModel : BaseViewModel
    {
        private PregnancyWeekContent _currentWeekData;

        public PregnancyWeekContent CurrentWeekData
        {
            get => _currentWeekData;
            set => SetProperty(ref _currentWeekData, value); 
        }

        private List<EducationCover> _pregEdCovers;

        public List<EducationCover> PregEdCovers
        {
            get => _pregEdCovers;
            set => SetProperty(ref _pregEdCovers, value);
        }


        private int _week;
        public int Week
        {
            get => _week;
            set
            {
                if (_week != value)
                {
                    _week = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _heroImage;
        public string HeroImage
        {
            get => _heroImage;
            set
            {
                if (_heroImage != value)
                {
                    _heroImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsMainUser { get; set; }

        public List<EducationCoverGroup> EducationGroupsCovers { get; set; }

        private readonly IPregnancyService _localPregdbServ;
        private readonly IEducationService _eduServ;

        public PregnancyDashboardViewModel(IPregnancyService localPregdbServ, IEducationService eduServ)
        {
            _localPregdbServ = localPregdbServ;
            _eduServ = eduServ;

            LoadPageDate();
        }

        private void LoadPageDate()
        {
            try
            {
                var userCycle = LocalStorageService.UserCycleProfile;
                Week = SharedCommonFunctions.GetCurrentPregnancyWeekFromLMP(userCycle.LastPeriodDate);
                EducationGroupsCovers = _eduServ.GetModuleEducationGroupsCoversByType(ModuleType.Pregnancy, Helpers.Enums.EducationCategoryType.Recommended);
                UpdateViewsForWeek();

                IsMainUser = LocalStorageService.UserDetails.UserRole == UserRoleType.Client;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to load pregnancy home page data, error: {ex.Message}");
            }
        }

        public void GoToPreviousWeek()
        {
            if (Week > 1)
            {
                Week--;
                UpdateViewsForWeek();
            }
        }

        public void GoToNextWeek()
        {
            if (Week < 41)
            {
                Week++;
                UpdateViewsForWeek();
            }
        }

        public void UpdateViewsForWeek()
        {
            try
            {
                HeroImage = $"p{Week}d.png";

                CurrentWeekData = _localPregdbServ.GetWeekPregnancyDataOnly(Week);
                PregEdCovers = _eduServ.GetCurrentWeekEducationCovers(ModuleType.Pregnancy, LocalStorageService.PregnancyData.PregnancyDataCurrentWeek);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to change home page pregnancy week data, error: {ex.Message}");
            }
        }

    }
}
