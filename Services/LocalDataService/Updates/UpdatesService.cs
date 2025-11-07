
using OvulaeApp.Helpers.Functions;
using OvulaeApp.ViewModels.Shared;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Models.Shared;
using OvulaeShared.Services.APIs.Interface;

namespace OvulaeApp.Services.LocalDataService.Updates
{
    public class UpdatesService : IUpdatesService
    {
        private readonly IWebInterfaceApiService _webAPI;

        private static List<UpdateHistory> AllServerUpdateHistory;
        private static List<UpdateHistory> AllLocalUpdateHistory;

        public UpdatesService(IWebInterfaceApiService webAPI)
        {
            _webAPI = webAPI;
        }

        public async Task<bool> ReloadUpdateHistoryDataAsync()
        {
            try
            {
                AllServerUpdateHistory = await _webAPI.GetData<List<UpdateHistory>>(OvulaeApiEndPoints.UPDATES.GET_ALL_HISTORY);
                AllLocalUpdateHistory = await FileReaderHelper.GetAllUpdateHistory();

                return true;
            }
            catch
            {
                AllServerUpdateHistory = new();
                AllLocalUpdateHistory = new();

                return false;
            }
        }

        public bool PerformServerUpdateModule(ModuleType moduleType)
        {
            var moduleUpdateServer = AllServerUpdateHistory.FirstOrDefault(x => x.ModuleName == moduleType.GetDisplayName());
            var moduleUpdateLocal = AllLocalUpdateHistory.FirstOrDefault(x => x.ModuleName == moduleType.GetDisplayName());

            return moduleUpdateServer?.LastUpdateTime > moduleUpdateLocal?.LastUpdateTime;
        }

        public bool PerformServerUpdateFeature(OvulaeFeatureType featureType)
        {
            var featureUpdateServer = AllServerUpdateHistory.FirstOrDefault(x => x.FeatureName == featureType.GetDisplayName());
            var featureUpdateLocal = AllLocalUpdateHistory.FirstOrDefault(x => x.FeatureName == featureType.GetDisplayName());

            return featureUpdateServer?.LastUpdateTime > featureUpdateLocal?.LastUpdateTime;
        }
    }
}
