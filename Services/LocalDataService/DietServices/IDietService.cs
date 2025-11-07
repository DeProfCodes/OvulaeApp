using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Diet;

namespace OvulaeApp.Services.LocalDataService.DietServices
{
    public interface IDietService
    {
        public List<DietGroupItemsViewModel> GetWeekDiet(ModuleType moduleType, int week);
    }
}
