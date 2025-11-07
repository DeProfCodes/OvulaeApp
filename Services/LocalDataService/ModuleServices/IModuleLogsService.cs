using OvulaeShared.Models.Menopause;
using OvulaeShared.Models.Ovulation;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.WebApi;

namespace OvulaeApp.Services.LocalDataService.ModuleServices
{
    public  interface IModuleLogsService
    {
        public Task<bool> LoadPregnancyLogs(string userId);

        public PregnancyLogEntryItem GetTodayPregnancyLog();

        public PregnancyLogEntryItem GetPregnancyLogByDate(DateTime date);

        public Task<GenericResult> UpdatePregnancyLog(PregnancyLogEntryItem updateTodayLog);

        public Task<bool> LoadPeriodLogs(string userId);

        public PeriodLogEntry GetTodayPeriodLog();

        public Task<GenericResult> UpdateTodayPeriodLog(PeriodLogEntry updateTodayLog);

        public Task<bool> LoadOvulationLogs(string userId);

        public OvulationCycleLog GetTodayOvulationLog();

        public Task<GenericResult> UpdateTodayOvulationLog(OvulationCycleLog updateTodayLog);

        public Task<bool> LoadMenopauseLogs(string userId);

        public MenopauseLogEntry GetTodayMenopauseLog();

        public Task<GenericResult> UpdateTodayMenopauseLog(MenopauseLogEntry updateTodayLog);

    }
}
