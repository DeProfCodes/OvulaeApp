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

        public PregnancyLogEntryItem GetPregnancyLogByEntryId(int entryId);

        public Task<GenericResult> UpdatePregnancyLog(PregnancyLogEntryItem updateTodayLog);

        public Task<bool> LoadPeriodLogs(string userId);

        public PeriodLogEntry GetTodayPeriodLog();

        public PeriodLogEntry GetPeriodLogByDate(DateTime date);

        public PeriodLogEntry GetPeriodLogByEntryId(int entryId);

        public Task<GenericResult> UpdateTodayPeriodLog(PeriodLogEntry updateTodayLog);

        public Task<bool> LoadOvulationLogs(string userId);

        public OvulationCycleLog GetTodayOvulationLog();

        public OvulationCycleLog GetOvulationLogByDate(DateTime date);

        public OvulationCycleLog GetOvulationLogByEntryId(int entryId);

        public Task<GenericResult> UpdateTodayOvulationLog(OvulationCycleLog updateTodayLog);

        public Task<bool> LoadMenopauseLogs(string userId);

        public MenopauseLogEntry GetTodayMenopauseLog();

        public MenopauseLogEntry GetMenopauseLogByDate(DateTime date);

        public MenopauseLogEntry GetMenopauseLogByEntryId(int entryId);

        public Task<GenericResult> UpdateTodayMenopauseLog(MenopauseLogEntry updateTodayLog);

    }
}
