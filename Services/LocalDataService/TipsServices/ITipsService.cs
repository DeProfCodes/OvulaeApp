using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.Services.LocalDataService.TipsServices
{
    public interface ITipsService
    {
        public Task<bool> ReloadTipsDataAsync();

        public List<TipsGroupItemsViewModel> GetModuleAllWeeksData(ModuleType moduleType);

        public TipsGroupItemsViewModel GetWeekTips(ModuleType moduleType, int week);

        public List<int> GetLikedTipsIds(ModuleType moduleType, int week);

        public Task<bool> SaveLikedTips(ModuleType moduleType, int week, List<int> likedTipsIds);
    }
}
