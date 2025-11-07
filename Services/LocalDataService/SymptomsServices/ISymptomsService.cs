using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Symptoms;

namespace OvulaeApp.Services.LocalDataService.SymptomsServices
{
    public interface ISymptomsService
    {
        public Task<bool> ReloadSymptomsDataAsync();

        public List<SymptomsGroupItemsViewModel> GetModuleAllWeeksData(ModuleType moduleType);

        public SymptomsGroupItemsViewModel GetWeekSymptoms(ModuleType moduleType, int week);
    }
}
