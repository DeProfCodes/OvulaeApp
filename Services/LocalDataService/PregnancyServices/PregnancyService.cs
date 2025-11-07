using System.Net;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.User;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Interface;
using OvulaeShared.ViewModel.Pregnancy;

namespace OvulaeApp.Services.LocalDataService.PregnancyServices
{
    public class PregnancyService : IPregnancyService
    {
        private readonly IWebInterfaceApiService webAPI;
        private readonly IUpdatesService _updatesServ;
        private readonly ICycleService _cycleServ;
        private ModuleType moduleType;

        private const string PregnancyFile = "PregnancyRawData.json";

        private List<PregnancyDataGroupViewModel> PregData => LocalStorageService.PregnancyData.PregnancyAllWeeksData;

        public PregnancyService(IUpdatesService updatesServ, ICycleService cycleServ, IWebInterfaceApiService webAPI)
        {
            _updatesServ = updatesServ;
            _cycleServ = cycleServ;
            this.webAPI = webAPI;

            moduleType = ModuleType.Pregnancy;
        }

        public async Task<bool> ReloadPregnancyDataAsync()
        {
            try
            {
                var data = await FileReaderHelper.GetModulaDashboardData(moduleType);

                if (_updatesServ.PerformServerUpdateModule(moduleType) || data == null)
                {
                    data = await webAPI.GetData<List<PregnancyDataGroupViewModel>>(OvulaeApiEndPoints.PREGNANCY.GET_PREGNANCY_DATA);
                    await FileReaderHelper.SaveDataAsync(data, PregnancyFile);
                }

                LocalStorageService.PregnancyData.PregnancyAllWeeksData = data;

                return true;
            }
            catch
            {
                LocalStorageService.PregnancyData.PregnancyAllWeeksData = null;
                return false;
            }
        }

        public int GetPregnancyCurrentWeek()
        {
            return _cycleServ.GetCurrentPregnancyWeekFromLMP();
        }

        public PregnancyDataGroupViewModel GetWeekPregnancyData(int week)
        {
            var data = PregData.FirstOrDefault(p => p.PregnancyInfo.Week == week);

            return data;
        }

        public List<PregnancyDataGroupViewModel> GetAllWeeksPregnancyData()
        {
            return PregData;
        }

        public PregnancyWeekContent GetWeekPregnancyDataOnly(int week)
        {
            var data = PregData.FirstOrDefault(p => p.PregnancyInfo.Week == week)?.PregnancyInfo;

            return data;
        }

        public List<PregnancyWeekContent> GetAllWeekPregnancyData()
        {
            var data = PregData.Select(p => p.PregnancyInfo).ToList();

            return data;
        }

        public PregnancyBabyData GetBabyDevelopmentForWeek(int week)
        {
            var data = PregData.FirstOrDefault(p => p.PregnancyInfo.Week == week)?.BabyDevelopmentDetails ?? new();

            return data;
        }
    }
}
