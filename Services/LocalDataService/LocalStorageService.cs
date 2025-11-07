using CommunityToolkit.Maui.Converters;
using OvulaeApp.Models.Authentication;
using OvulaeApp.Models.Shared;
using OvulaeApp.ViewModels.GoalSetting;
using OvulaeApp.ViewModels.Tips;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Models.Menopause;
using OvulaeShared.Models.Ovulation;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.User;
using OvulaeShared.ViewModel.Affiliates;
using OvulaeShared.ViewModel.Diet;
using OvulaeShared.ViewModel.Education;
using OvulaeShared.ViewModel.Pregnancy;
using OvulaeShared.ViewModel.Symptoms;
using OvulaeShared.ViewModel.Tips;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Services.LocalDataService
{
    public class LocalStorageService
    {
        public static UserModel UserDetails { get; set; } = new();

        public static UserRoleType LoggedInRole { get; set; }

        public static PasswordReset PasswordReset { get; set; } = new();

        public static bool Authenticated { get; set; } = false;

        public static ModuleType AppPrimaryGoal { get; set; }

        public static class PregnancyData
        {
            public static int PregnancyDataCurrentWeek { get; set; }

            public static List<PregnancyDataGroupViewModel> PregnancyAllWeeksData { get; set; } = new();

            public static List<EducationGroupItemsViewModel> AllEducationBooks { get; set; } = new(); 

            public static List<DietGroupItemsViewModel> AllPregDietGroups { get; set;} = new();

            public static List<SymptomsGroupItemsViewModel> AllPregSymptomsGroups { get; set; } = new();

            public static List<TipsGroupItemsViewModel> AllPregTips { get; set; } = new();

            public static List<LikedTipsViewModel> LikedPregnancyTips { get; set; } = new();

            public static DateTime BabyBirthDate { get; set; }

            public static string BabyName { get; set; }

            public static string BabyGender { get; set; }

            public static DateTime MiscarriageDate { get; set; }
        }

        public static class ModuleLogsData
        {
            public static PregnancyTrackerLog PregnancyLogs { get; set; } = new();

            public static PeriodTrackerLog PeriodLogs { get; set; } = new();

            public static OvulationTrackerLog OvulationLogs { get; set; } = new();

            public static MenopauseTrackerLog MenopauseLogs { get; set; } = new();
        }

        public static bool PeriodTrackerSet { get; set; } = true;

        public static OvulaeDoctor OvulaeGynacologist { get; set; } = new OvulaeDoctor { Firstname = "E", Lastname = "Wypkema" };

        public static UserBodyMetric UserBodyMetrics { get; set; } = new();

        public static GoalSettingDataViewModel OnboardingData {get; set;} = new();

        public static UserCycleProfile UserCycleProfile { get; set; } = new();

        public static UserSubscription UserSubscription { get; set; } = new();

        public static UserPartnerViewModel PartnerDetails { get; set; }

        public static AffiliateOverviewDetails AffiliateOverviewDetails { get; set; }

        public static bool IS_TESTING_MODE { get; set; } = false;

        public static DateTime TempCalculatedLMP { get; set; }

        public static UserBodyMetric TempBodyMetrics { get; set; } = new();

        public static UserModel TempUserDetails { get; set; } = new();

        public static string AffiliateJoinCode { get; set; }

        public static MobileDeviceType MobileDeviceType { get; set; }
    }
}