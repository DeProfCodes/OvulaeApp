using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Services.LocalDataService.SymptomsServices;
using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Symptoms;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancySymptomsViewModel
    {
        public SymptomsGroupItemsViewModel MainData { get; set; } 

        public string OurGynacologist { get; set; }

        private readonly ISymptomsService _symptomsServ;
        private readonly int _week;

        public PregnancySymptomsViewModel(ISymptomsService symptomsServ, int week)
        {
            _symptomsServ = symptomsServ;
            _week = week;
        }

        public async Task InitializeAsync()
        {
            // Run filtering off UI thread
            MainData = await Task.Run(() => _symptomsServ.GetWeekSymptoms(ModuleType.Pregnancy, _week));

            var gyn = LocalStorageService.OvulaeGynacologist;
            OurGynacologist = $"Dr. {gyn.Firstname} {gyn.Lastname}, OB/GYN";
        }


    }
}
