using OvulaeApp.Helpers.Functions;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.Services.Notifications
{
    public class NotificationsPreferenceService : INotificationsPreferenceService
    {
        private readonly INotificationSchedulerService _notificationSchedulerService;
        private readonly ICycleService _cycleServ;
        private readonly IPregnancyService _pregnancyServ;
        private readonly ISymptomsService _symptomsServ;
        private readonly ITipsService _tipsServ;
        private readonly IEducationService _eduServ;
        private readonly IUserLocalService _userServ;
        private readonly IMenopauseService _menopauseServ;

        private readonly PregnancyHelper pregHelper;

        private NotificationsGroup _allNotifications;
        private const string NotificationsFile = "NotificationsPreferences.json";

        public NotificationsPreferenceService(INotificationSchedulerService notificationSchedulerService, ICycleService cycleServ, IUserLocalService usersServ, 
                                              IPregnancyService pregnancyServ, ISymptomsService symptomsServ, ITipsService tipsServ, IEducationService eduServ,
                                              IMenopauseService menopauseServ)
        {
            _notificationSchedulerService = notificationSchedulerService;
            _userServ = usersServ;
            _cycleServ = cycleServ;
            _pregnancyServ = pregnancyServ;
            _symptomsServ = symptomsServ;
            _tipsServ = tipsServ;
            _eduServ = eduServ;
            _menopauseServ = menopauseServ;

            pregHelper = new PregnancyHelper();
        }

        private async Task EnsureLoadedAsync()
        {
            if (_allNotifications == null)
            {
                var preferences = await FileReaderHelper.GetAllFileData<NotificationsGroup>(NotificationsFile);
                _allNotifications = preferences ?? new NotificationsGroup
                {
                    PregnancyNotifications = DefaultValueHelper.CreateWithDefaults<PregnancyNotificationPreferences>(),
                    PeriodOvulationNotification = DefaultValueHelper.CreateWithDefaults<PeriodOvulationNotificationPreferences>()
                };

                await SaveNotificationsPreferences();
            }
        }

        private async Task<bool> SaveNotificationsPreferences()
        {
            return _allNotifications != null
                && await FileReaderHelper.SaveDataAsync(_allNotifications, NotificationsFile);
        }

        public async Task<PeriodOvulationNotificationPreferences> GetPeriodOvulationNotifications()
        {
            await EnsureLoadedAsync();
            return _allNotifications?.PeriodOvulationNotification ?? new PeriodOvulationNotificationPreferences();
        }

        public async Task<PregnancyNotificationPreferences> GetPregnancyNotifications()
        {
            await EnsureLoadedAsync();
            return _allNotifications?.PregnancyNotifications ?? new PregnancyNotificationPreferences();
        }

        public async Task<bool> UpdatePeriodOvulationNotifications(PeriodOvulationNotificationPreferences newPreferences)
        {
            await EnsureLoadedAsync();

            _allNotifications.PeriodOvulationNotification = newPreferences;
            var saved = await SaveNotificationsPreferences();

            if (saved)
            {
                _notificationSchedulerService.ClearScheduledNotifications();

                await _cycleServ.LoadCycleDataAsync();
                var plan = _cycleServ.BuildDayPlan(DateTime.Today, newPreferences);
                await _notificationSchedulerService.SchedulePlanAsync(plan);
            }

            return saved;
        }

        private async Task ReloadPregnancyNotificationsData()
        {
            try
            {
                var userProfileLoad = await _userServ.GetUserProfile();
                var pregnancyDataLoad = await _pregnancyServ.ReloadPregnancyDataAsync();
                var pregSymptomsLoad = await _symptomsServ.ReloadSymptomsDataAsync();
                var pregTips = await _tipsServ.ReloadTipsDataAsync();
                var educationLoad = await _eduServ.ReloadEducationDataAsync();
            }
            catch
            {
                
            }
        }

        public async Task<bool> UpdatePregnancyNotifications(PregnancyNotificationPreferences newPreferences)
        {
            await EnsureLoadedAsync();
            _allNotifications.PregnancyNotifications = newPreferences;
            var saved = await SaveNotificationsPreferences();

            if (saved)
            {
                _notificationSchedulerService.ClearScheduledNotifications();

                await ReloadPregnancyNotificationsData();

                var userCycle = await _userServ.GetUserCycleDetails();
                var pregData = _pregnancyServ.GetAllWeeksPregnancyData();
                var symptoms = _symptomsServ.GetModuleAllWeeksData(ModuleType.Pregnancy);
                var tips = _tipsServ.GetModuleAllWeeksData(ModuleType.Pregnancy);
                var education = _eduServ.GetModuleEducationGroupsCoversByType(ModuleType.Pregnancy);

                var plan = pregHelper.BuildFullWeekPlan(DateTime.Today, userCycle.LastPeriodDate, newPreferences, pregData, education, symptoms, tips);
                await _notificationSchedulerService.SchedulePlanAsync(plan);
            }

            return saved;
        }

        public async Task<MenopauseNotificationPreferences> GetMenopauseNotifications()
        {
            await EnsureLoadedAsync();
            return _allNotifications?.MenopauseNotification ?? new MenopauseNotificationPreferences();
        }

        public async Task<bool> UpdateMenopauseNotifications(MenopauseNotificationPreferences newPreferences)
        {
            await EnsureLoadedAsync();

            _allNotifications.MenopauseNotification = newPreferences;
            var saved = await SaveNotificationsPreferences();

            if (saved)
            {
                _notificationSchedulerService.ClearScheduledNotifications();

                var plan = _menopauseServ.BuildDayPlan(DateTime.Today, newPreferences);
                await _notificationSchedulerService.SchedulePlanAsync(plan);
            }

            return saved;
        }

        public async Task<List<ScheduledNotification>> BuildFullWeekPlanForPregnancyNotifications()
        {
            try
            {
                await ReloadPregnancyNotificationsData();

                var userCycle = await _userServ.GetUserCycleDetails();
                var pregData = _pregnancyServ.GetAllWeeksPregnancyData();
                var symptoms = _symptomsServ.GetModuleAllWeeksData(ModuleType.Pregnancy);
                var tips = _tipsServ.GetModuleAllWeeksData(ModuleType.Pregnancy);
                var education = _eduServ.GetModuleEducationGroupsCoversByType(ModuleType.Pregnancy);

                var prefs = await GetPregnancyNotifications();

                return pregHelper.BuildFullWeekPlan(DateTime.Today, userCycle.LastPeriodDate, prefs, pregData, education, symptoms, tips);
            }
            catch
            {
                return new();                
            }
        }
    }
}
