using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.WebApi;
using OvulaeShared.ViewModel.Pregnancy;

namespace OvulaeApp.Services.LocalDataService.PregnancyServices
{
    public interface IPregnancyService
    {
        public Task<bool> ReloadPregnancyDataAsync();

        public int GetPregnancyCurrentWeek();

        public PregnancyDataGroupViewModel GetWeekPregnancyData(int week);

        public List<PregnancyDataGroupViewModel> GetAllWeeksPregnancyData();

        public PregnancyWeekContent GetWeekPregnancyDataOnly(int week);

        public List<PregnancyWeekContent> GetAllWeekPregnancyData();

        public PregnancyBabyData GetBabyDevelopmentForWeek(int week);


    }
}
