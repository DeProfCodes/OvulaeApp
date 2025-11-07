using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService.MenopauseServices;
using OvulaeApp.Services.LocalDataService.ModuleServices;
using OvulaeApp.Services.LocalDataService.PeriodTrackerServices;
using OvulaeApp.Services.LocalDataService.PregnancyServices;
using OvulaeApp.Services.LocalDataService.Subscription;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeShared.Enums.App;
using OvulaeShared.Services.APIs.Interface;

namespace OvulaeApp.Services.LocalDataService
{
    public class LocalDbService : ILocalDbService
    {
        private readonly IEducationService _eduServ;
        private readonly IUserLocalService _usersServ;
        private readonly IPregnancyService _pregServ;
        private readonly ISymptomsService _symptomsServ;
        private readonly ITipsService _tipsServ;
        private readonly IUpdatesService _updatesServ;
        private readonly IModuleLogsService _moduleLogsServ;
        private readonly ISubscriptionService _subscriptionServ;
        private readonly IWebInterfaceApiService _webApi;

        public LocalDbService(IEducationService eduServ, IUserLocalService usersServ, IPregnancyService pregServ, ISymptomsService symptomsServ, 
                              ITipsService tipsServ, IUpdatesService updatesServ, IModuleLogsService moduleLogsServ, ISubscriptionService subscriptionServ,
                              IWebInterfaceApiService webApi)
        {
            _eduServ = eduServ;
            _usersServ = usersServ;
            _pregServ = pregServ;
            _symptomsServ = symptomsServ;   
            _tipsServ = tipsServ;
            _updatesServ = updatesServ;
            _moduleLogsServ = moduleLogsServ;
            _subscriptionServ = subscriptionServ;
            //_webApi = webApi;
        }

        private async Task<bool> LoadModuleLogs()
        {
            var moduleType = LocalStorageService.AppPrimaryGoal;
            var userId = LocalStorageService.UserDetails.UserId;

            if (string.IsNullOrEmpty(userId)) return false;

            if (moduleType == ModuleType.Pregnancy)
            {
                return await _moduleLogsServ.LoadPregnancyLogs(userId);
            }
            if (moduleType == ModuleType.PeriodTracker)
            {
                return await _moduleLogsServ.LoadPeriodLogs(userId);
            }
            if (moduleType == ModuleType.Ovulation)
            {
                return await _moduleLogsServ.LoadOvulationLogs(userId);
            }
            if (moduleType == ModuleType.MenopauseTracker && !string.IsNullOrEmpty(userId))
            {
                return await _moduleLogsServ.LoadMenopauseLogs(userId);
            }
            return false;
        }

        public async Task<bool> LoadStartupData()
        {
            try
            {
                var updatesLoad = await _updatesServ.ReloadUpdateHistoryDataAsync();
                var userProfileLoad = await _usersServ.GetUserProfile();
                var pregnancyLoad = await _pregServ.ReloadPregnancyDataAsync();
                var pregTipsLoad = await _tipsServ.ReloadTipsDataAsync();
                var symptomsLoad = await _symptomsServ.ReloadSymptomsDataAsync();
                var educationLoad = await _eduServ.ReloadEducationDataAsync();
                var loadLogs = await LoadModuleLogs();

                if (!string.IsNullOrEmpty(LocalStorageService.AffiliateJoinCode))
                {
                    await _usersServ.SaveAffiliateJoinLink();
                }
                else
                {
                    await _usersServ.LoadAffiliateJoinLink();
                }

                await _subscriptionServ.RunSubscriptionCheck();

                var allLoadedSuccess = updatesLoad && userProfileLoad && pregnancyLoad && pregTipsLoad && symptomsLoad && educationLoad && loadLogs;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
