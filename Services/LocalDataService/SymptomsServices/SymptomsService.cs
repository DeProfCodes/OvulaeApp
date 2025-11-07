using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Services.APIs.Interface;
using OvulaeShared.ViewModel.Symptoms;

namespace OvulaeApp.Services.LocalDataService.SymptomsServices
{
    public class SymptomsService : ISymptomsService
    {
        private readonly IWebInterfaceApiService _webAPI;
        private readonly IUpdatesService _updatesServ;

        private const string SymptomsFile = "PregSymptomsRawData.json";

        public SymptomsService(IUpdatesService updatesServ, IWebInterfaceApiService webApi)
        {
            _updatesServ = updatesServ;
            _webAPI = webApi;
        }

        public async Task<bool> ReloadSymptomsDataAsync()
        {
            try
            {
                var data = await FileReaderHelper.GetAllFileData<List<SymptomsGroupItemsViewModel>>(SymptomsFile);

                if (_updatesServ.PerformServerUpdateFeature(OvulaeFeatureType.Symptoms) || data == null)
                {
                    data = await _webAPI.GetData<List<SymptomsGroupItemsViewModel>>($"{OvulaeApiEndPoints.SYMPTOMS.GET_SYMPTOMS_DATA}?moduleType=All");
                    await FileReaderHelper.SaveDataAsync(data, SymptomsFile);
                }

                LocalStorageService.PregnancyData.AllPregSymptomsGroups = data;
                return true;
            }
            catch
            {
                LocalStorageService.PregnancyData.AllPregSymptomsGroups = null;
                return false;
            }
        }

        public List<SymptomsGroupItemsViewModel> GetModuleAllWeeksData(ModuleType moduleType)
        {
            var data = new List<SymptomsGroupItemsViewModel>();

            if (moduleType == ModuleType.Pregnancy)
                data = LocalStorageService.PregnancyData.AllPregSymptomsGroups;

            return data;
        }

        public SymptomsGroupItemsViewModel GetWeekSymptoms(ModuleType moduleType, int week)
        {
            var data = GetModuleAllWeeksData(moduleType).FirstOrDefault(d => $"{week}" == d.SymptomsGroup.Weeks);
            
            return data;
        }
    }
}
