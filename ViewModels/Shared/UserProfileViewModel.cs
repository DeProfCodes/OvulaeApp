using System.Diagnostics.Metrics;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.ViewModels.Authentication;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.HealthProfile;
using OvulaeShared.Enums.User;

namespace OvulaeApp.ViewModels.Shared
{
    public class UserProfileViewModel : BaseViewModel
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "profile2_icon.png";

        public bool IsPremium { get; set; } = false;
        public bool IsDoctor { get; set; } = false;
        public bool IsAffiliatePartner { get; set; } = false;

        public int BirthYear { get; set; } = 0;
        public string Country { get; set; } = "-";

        public string GestationalAge { get; set; } = "";
        public string DueDate { get; set; } = "";
        public string BabyGender { get; set; } = "Girl";

        public double Weight { get; set; } = 0;
        public string WeightMeasure { get; set; } = "";

        public double Height { get; set; } = 0;
        public string HeightMeasure { get; set; } = "";

        public string BloodGroup { get; set; } = "";

        public string LastPeriodDate { get; set; } = "";
        public string CycleLength { get; set; } = "";
        public string PeriodLength { get; set; } = "";
        public string PrimaryGoal { get; set; } = "";
        public string FertileWindow { get; set; } = "";

        public string HormonalTreatment { get; set; }
        public string IrregularPeriods { get; set; }
        public string SkippedPeriods { get; set; }
        public string HotFlashes { get; set; }
        public string NightSweats { get; set; }
        public string MoodSwings { get; set; }

        public bool ShowPregnancyInfo { get; set; } = false;
        public bool ShowPeriodTrackerInfo { get; set; } = false;
        public bool ShowOvulationInfo { get; set; } = false;
        public bool ShowMenopauseInfo { get; set; } = false;

        private readonly ICycleService _cycleServ;

        public UserProfileViewModel(ICycleService cycleServ)
        {
            _cycleServ = cycleServ;
            SetData();
        }

        private void SetData()
        {
            try
            {
                if (LocalStorageService.UserDetails != null && LocalStorageService.UserBodyMetrics != null)
                {
                    var alias = LocalStorageService.UserDetails;
                    var authed = LocalStorageService.Authenticated;
                    var cycleProfile = LocalStorageService.UserCycleProfile;

                    FullName = authed ? $"{alias.Firstname} {alias.Lastname}" : "-";
                    Email = authed ? alias.Email : "-";
                    PhoneNumber = authed ? alias.PhoneNumber : "-";
                    Country = PhoneEntryViewModel.GetCountryNameFromCode(alias.CountryCode);

                    BirthYear = authed ? LocalStorageService.UserBodyMetrics.Year : 0;
                    Weight = authed ? LocalStorageService.UserBodyMetrics.Weight : 0;
                    WeightMeasure = authed ? LocalStorageService.UserBodyMetrics.WeightUnit : "kg";
                    Height = authed ? LocalStorageService.UserBodyMetrics.Height : 0;
                    HeightMeasure = authed ? LocalStorageService.UserBodyMetrics.HeightUnit : "cm";

                    if (!string.IsNullOrEmpty(LocalStorageService.UserBodyMetrics.BloodGroup) && !string.IsNullOrEmpty(LocalStorageService.UserBodyMetrics.RhFactor))
                    {
                        BloodGroup = $"{LocalStorageService.UserBodyMetrics.BloodGroup}{LocalStorageService.UserBodyMetrics.RhFactor}"; 
                    }

                    //IsPremium = LocalStorageService.LoggedInRole == UserRoleType.Client;
                    //IsDoctor = LocalStorageService.LoggedInRole == UserRoleType.Doctor;
                    //IsAffiliatePartner = LocalStorageService.LoggedInRole == UserRoleType.Affiliate;

                    var moduleType = cycleProfile.OvulaePrimaryGoal;

                    if (moduleType == ModuleType.Pregnancy.GetDisplayName())
                    {
                        GestationalAge = $"{LocalStorageService.PregnancyData.PregnancyDataCurrentWeek} Weeks";
                        DueDate = $"{SharedCommonFunctions.GetPregnancyDueDateFromLMP(cycleProfile.LastPeriodDate.Value)}:dd MMMM yyyy";
                        BabyGender = "Girl";
                        ShowPregnancyInfo = true;
                    }
                    else
                    {
                        LastPeriodDate = cycleProfile.LastPeriodDate?.ToString("dd MMM yyyy") ?? "-";
                        CycleLength = $"{cycleProfile.CycleLengthDays ?? 0} days";
                        PeriodLength = $"{cycleProfile.PeriodLengthDays ?? 0} days";
                        ShowPeriodTrackerInfo = moduleType == ModuleType.PeriodTracker.GetDisplayName();
                    }
                    
                    if (moduleType == ModuleType.Ovulation.GetDisplayName())
                    {
                        PrimaryGoal = cycleProfile.OvulaePrimaryGoal;

                        var (fertileStart, fertileEnd) = _cycleServ.GetNextFertileWindow();
                        FertileWindow = $"{fertileStart:dd MMM} - {fertileEnd:dd MMM}";
                        ShowOvulationInfo = true;
                    }
                    else if (moduleType == ModuleType.MenopauseTracker.GetDisplayName())
                    {
                        HormonalTreatment = cycleProfile.Treatments.Contains(TreatmentTypes.HRT.GetDisplayDescription()) ? "Yes" : "No";
                        IrregularPeriods = cycleProfile.PeriodIrregularityType == PeriodIrregularityType.OccasionallyIrregular || cycleProfile.PeriodIrregularityType == PeriodIrregularityType.FrequentlyIrregular ? "Yes" : "No";
                        SkippedPeriods = cycleProfile.PeriodIrregularityType == PeriodIrregularityType.FrequentlyIrregular ? "Yes" : "No";
                        HotFlashes = cycleProfile.Symptoms.Contains(SymptomsTypes.HotFlashes.GetDisplayName()) ? "Yes" : "No";
                        NightSweats = cycleProfile.Symptoms.Contains(SymptomsTypes.NightSweats.GetDisplayName()) ? "Yes" : "No";
                        MoodSwings = cycleProfile.Symptoms.Contains(SymptomsTypes.MoodChanges.GetDisplayName()) ? "Yes" : "No";

                        ShowMenopauseInfo = true;
                    }
                }
            }
            catch
            {
                FullName = "-";
                Email = "-";
                PhoneNumber = "-";
                GestationalAge = $"-";

                BirthYear = 0;
                Weight = 0;
                WeightMeasure = "-";
                Height = 0;
                HeightMeasure = "-";

                IsPremium = true;
                IsDoctor = false;
                IsAffiliatePartner = false;
                DueDate = "-";

            }
        }
    }
}
