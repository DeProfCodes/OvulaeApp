using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeApp.ViewModels.Tips;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Services.APIs.Interface;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.Services.LocalDataService.TipsServices
{
    public class TipsService : ITipsService
    {
        private readonly IWebInterfaceApiService _webAPI;
        private readonly IUpdatesService _updatesServ;

        private const string TipsFile = "PregTipsRawData.json";
        private const string TipsLocalFileName = "UserLikedTips.json";

        public TipsService(IUpdatesService updatesServ, IWebInterfaceApiService webApi)
        {
            _updatesServ = updatesServ;
            _webAPI = webApi;
        }

        public async Task<bool> ReloadTipsDataAsync()
        {
            try
            {
                var data = await FileReaderHelper.GetAllFileData<List<TipsGroupItemsViewModel>>(TipsFile);

                if (_updatesServ.PerformServerUpdateFeature(OvulaeFeatureType.Tips) || data == null)
                {
                    data = await _webAPI.GetData<List<TipsGroupItemsViewModel>>($"{OvulaeApiEndPoints.TIPS.GET_TIPS_DATA}?moduleType=All");
                    await FileReaderHelper.SaveDataAsync(data, TipsFile);
                }

                var likedPregTips = await FileReaderHelper.GetAllFileData<List<LikedTipsViewModel>>(TipsLocalFileName);

                if (likedPregTips == null)
                {
                    likedPregTips = new();
                    await FileReaderHelper.SaveDataAsync(data, TipsLocalFileName);
                }

                LocalStorageService.PregnancyData.AllPregTips = data;
                LocalStorageService.PregnancyData.LikedPregnancyTips = likedPregTips?.Where(x => x.ModuleType == ModuleType.Pregnancy).ToList() ?? new();

                return true;
            }
            catch
            {
                LocalStorageService.PregnancyData.AllPregTips = null;
                LocalStorageService.PregnancyData.LikedPregnancyTips = null;

                return false;
            }
        }

        public List<TipsGroupItemsViewModel> GetModuleAllWeeksData(ModuleType moduleType)
        {
            var data = new List<TipsGroupItemsViewModel>();

            if (moduleType == ModuleType.Pregnancy)
                data = LocalStorageService.PregnancyData.AllPregTips;

            return data;
        }

        public TipsGroupItemsViewModel GetWeekTips(ModuleType moduleType, int week)
        {
            var data = GetModuleAllWeeksData(moduleType).FirstOrDefault(d => $"{week}" == d.TipsGroup.Weeks);
            
            return data;
        }

        public List<int> GetLikedTipsIds(ModuleType moduleType, int week)
        {
            try
            {
                var tipsIds = LocalStorageService.PregnancyData.LikedPregnancyTips.FirstOrDefault(t => t.Week == week)?.LikedTipIds ?? new();

                return tipsIds;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<bool> SaveLikedTips(ModuleType moduleType, int week, List<int> likedTipsIds)
        {
            try
            {
                var likedTipVm = new LikedTipsViewModel
                {
                    UserId = LocalStorageService.UserDetails.UserId,
                    ModuleType = moduleType,
                    Week = week,
                    LikedTipIds = likedTipsIds
                };

                var weekLiked = LocalStorageService.PregnancyData.LikedPregnancyTips.FirstOrDefault(t => t.Week == week);
                
                if(weekLiked != null)
                    weekLiked.LikedTipIds = likedTipsIds;

                else
                    LocalStorageService.PregnancyData.LikedPregnancyTips.Add(likedTipVm);

                var saveStatus = await FileReaderHelper.SaveDataAsync(LocalStorageService.PregnancyData.LikedPregnancyTips, TipsLocalFileName);

                return saveStatus;
            }
            catch
            {
                
            }
            return false;
        }
    }
}
