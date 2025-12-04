using OvulaeApp.Helpers.Enums;
using OvulaeApp.Models.Notifications;
using OvulaeApp.ViewModels.Module;
using OvulaeShared.Enums.ModuleEnums;
using OvulaeShared.ViewModel.Module;

namespace OvulaeApp.Services.LocalDataService.MenopauseServices
{
    public interface IMenopauseService
    {
        public MenopauseStage GetMenopauseStage();

        public ModulePhaseDataViewModel GetCurrentPhaseData();

        public ModulePhaseDataViewModel GetCurrentPhaseDataForStage(MenopauseStage stage);

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetails();

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetailsForStage(MenopauseStage stage);

        public List<ScheduledNotification> BuildDayPlan(DateTime date, MenopauseNotificationPreferences prefs);

        public List<ModulePhaseDataViewModel> GetAllPhaseData();
    }
}
