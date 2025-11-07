using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.TipsServices;
using OvulaeApp.ViewModels.Tips;
using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyTipsViewModel
    {
        public string Title { get; set; }
        
        public TipsGroupItemsViewModel MainData { get; set; }

        public List<TipViewModel> TipsData { get; set; }

        public string DrDetails { get; set; }

        public string Gynacologist { get; set; }

        public PregnancyTipsViewModel(ITipsService tipsServ, int week)
        {
            MainData = tipsServ.GetWeekTips(ModuleType.Pregnancy, week);
            TipsData = GetTipsViewModel(tipsServ, week);

            Title = $"Here are some of the TIPS for Week {week}";
            DrDetails = $"🩺 Tip from Dr. {LocalStorageService.OvulaeGynacologist.Firstname}";
            Gynacologist = $"Dr. {LocalStorageService.OvulaeGynacologist.Firstname} {LocalStorageService.OvulaeGynacologist.Lastname}, OB/GYN";
        }

        private List<TipViewModel> GetTipsViewModel(ITipsService tipsServ, int week)
        {
            var likedTips = tipsServ.GetLikedTipsIds(ModuleType.Pregnancy, week);

            var result = new List<TipViewModel>();
            foreach (var tip in MainData.TipItems)
            {
                result.Add(new TipViewModel
                {
                    TipItem = tip,
                    IsChecked = likedTips.Contains(tip.TipId)
                });
            }
            return result;
        }
    }
}
