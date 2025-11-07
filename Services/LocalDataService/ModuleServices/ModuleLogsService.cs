using OvulaeShared.Models.Menopause;
using OvulaeShared.Models.Ovulation;
using OvulaeShared.Models.PeriodTracker;
using OvulaeShared.Models.Pregnancy;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.ModuleServices;

namespace OvulaeApp.Services.LocalDataService.ModuleServices
{
    public class ModuleLogsService : IModuleLogsService
    {
        private readonly IModuleLogsApi _moduleLogsApi;
        private static PregnancyTrackerLog PregnancyLogs => LocalStorageService.ModuleLogsData.PregnancyLogs;
        private static PeriodTrackerLog PeriodLogs => LocalStorageService.ModuleLogsData.PeriodLogs;
        private static OvulationTrackerLog OvulationLogs => LocalStorageService.ModuleLogsData.OvulationLogs;
        private static MenopauseTrackerLog MenopauseLogs => LocalStorageService.ModuleLogsData.MenopauseLogs;

        private static string UserID => LocalStorageService.UserDetails.UserId;

        public ModuleLogsService(IModuleLogsApi moduleLogsApi)
        {
            _moduleLogsApi = moduleLogsApi;
        }

        public async Task<bool> LoadPregnancyLogs(string userId)
        {
            try
            {
                var logs = await _moduleLogsApi.GetPregnancyLogs(userId);

                LocalStorageService.ModuleLogsData.PregnancyLogs = logs ?? new();

                return logs != null;
            }
            catch
            {
                LocalStorageService.ModuleLogsData.MenopauseLogs = null;
            }
            return false;
        }

        public PregnancyLogEntryItem GetTodayPregnancyLog()
        {
            try
            {
                if (PregnancyLogs != null && PregnancyLogs.Id > 0)
                {
                    var todayLog = PregnancyLogs.Entries.FirstOrDefault(x => x.LogDate.Date == DateTime.Today.Date);

                    return todayLog;
                }
            }
            catch
            {
            }
            return null;
        }

        public PregnancyLogEntryItem GetPregnancyLogByDate(DateTime date)
        {
            try
            {
                if (PregnancyLogs != null && PregnancyLogs.Id > 0)
                {
                    var log = PregnancyLogs.Entries.FirstOrDefault(x => x.LogDate.Date == date.Date);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pregnancy log for date {date}: {ex.Message}");
            }
            return null;
        }

        public PregnancyLogEntryItem GetPregnancyLogByEntryId(int entryId)
        {
            try
            {
                if (PregnancyLogs != null && PregnancyLogs.Id > 0)
                {
                    var log = PregnancyLogs.Entries.FirstOrDefault(x => x.EntryId == entryId);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pregnancy log for Id {entryId}: {ex.Message}");
            }
            return null;
        }

        public async Task<GenericResult> UpdatePregnancyLog(PregnancyLogEntryItem updateTodayLog)
        {
            try
            {
                updateTodayLog.PregnancyTrackerLogId = (PregnancyLogs != null ) ? PregnancyLogs.Id : 0;
                var updateRes = await _moduleLogsApi.UpdatePregnancyLogEntry(UserID, updateTodayLog);

                if (updateRes.Success)
                    await LoadPregnancyLogs(UserID);

                return updateRes;
            }
            catch
            {
                return new();
            }
        }

        public async Task<bool> LoadPeriodLogs(string userId)
        {
            try
            {
                var logs = await _moduleLogsApi.GetPeriodLogs(userId);

                LocalStorageService.ModuleLogsData.PeriodLogs = logs ?? new();

                return logs != null;
            }
            catch
            {
                LocalStorageService.ModuleLogsData.MenopauseLogs = null;
            }
            return false;
        }

        public PeriodLogEntry GetTodayPeriodLog()
        {
            try
            {
                if (PeriodLogs != null && PeriodLogs.Id > 0)
                {
                    var todayLog = PeriodLogs.Logs.FirstOrDefault(x => x.LogDate.Date == DateTime.Today.Date);

                    return todayLog;
                }
            }
            catch
            {
            }
            return null;
        }

        public PeriodLogEntry GetPeriodLogByDate(DateTime date)
        {
            try
            {
                if (PeriodLogs != null && PeriodLogs.Id > 0)
                {
                    var log = PeriodLogs.Logs.FirstOrDefault(x => x.LogDate.Date == date.Date);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting period log for date {date}: {ex.Message}");
            }
            return null;
        }

        public PeriodLogEntry GetPeriodLogByEntryId(int entryId)
        {
            try
            {
                if (PeriodLogs != null && PeriodLogs.Id > 0)
                {
                    var log = PeriodLogs.Logs.FirstOrDefault(x => x.EntryId == entryId);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting period log for Id {entryId}: {ex.Message}");
            }
            return null;
        }

        public async Task<GenericResult> UpdateTodayPeriodLog(PeriodLogEntry updateTodayLog)
        {
            try
            {
                updateTodayLog.PeriodTrackerLogId = (PeriodLogs != null) ? PeriodLogs.Id : 0;
                var updateRes = await _moduleLogsApi.UpdatePeriodLogEntry(UserID, updateTodayLog);

                if(updateRes.Success)
                    await LoadPeriodLogs(UserID);

                return updateRes;
            }
            catch
            {
                return new();
            }
        }

        public async Task<bool> LoadOvulationLogs(string userId)
        {
            try
            {
                var logs = await _moduleLogsApi.GetOvulationLogs(userId);

                LocalStorageService.ModuleLogsData.OvulationLogs = logs ?? new();

                return logs != null;
            }
            catch
            {
                LocalStorageService.ModuleLogsData.MenopauseLogs = null;
            }
            return false;
        }

        public OvulationCycleLog GetTodayOvulationLog()
        {
            try
            {
                if (OvulationLogs != null && OvulationLogs.Id > 0)
                {
                    var todayLog = OvulationLogs.CycleTrackingHistory.FirstOrDefault(x => x.LogDate.Date == DateTime.Today.Date);

                    return todayLog;
                }
            }
            catch
            {
            }
            return null;
        }

        public OvulationCycleLog GetOvulationLogByDate(DateTime date)
        {
            try
            {
                if (OvulationLogs != null && OvulationLogs.Id > 0)
                {
                    var log = OvulationLogs.CycleTrackingHistory.FirstOrDefault(x => x.LogDate.Date == date.Date);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ovulation log for date {date}: {ex.Message}");
            }
            return null;
        }

        public OvulationCycleLog GetOvulationLogByEntryId(int entryId)
        {
            try
            {
                if (OvulationLogs != null && OvulationLogs.Id > 0)
                {
                    var log = OvulationLogs.CycleTrackingHistory.FirstOrDefault(x => x.EntryId == entryId);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ovulation log for Id {entryId}: {ex.Message}");
            }
            return null;
        }

        public async Task<GenericResult> UpdateTodayOvulationLog(OvulationCycleLog updateTodayLog)
        {
            try
            {
                updateTodayLog.OvulationTrackerLogId = (OvulationLogs != null) ? OvulationLogs.Id : 0;
                var updateRes = await _moduleLogsApi.UpdateOvulationLogEntry(UserID, updateTodayLog);

                if (updateRes.Success)
                    await LoadOvulationLogs(UserID);

                return updateRes;
            }
            catch
            {
                return new();
            }
        }

        public async Task<bool> LoadMenopauseLogs(string userId)
        {
            try
            {
                var logs = await _moduleLogsApi.GetMenopauseLogs(userId);

                LocalStorageService.ModuleLogsData.MenopauseLogs = logs ?? new();

                return logs != null;
            }
            catch
            {
                LocalStorageService.ModuleLogsData.MenopauseLogs = null;
            }
            return false;
        }

        public MenopauseLogEntry GetTodayMenopauseLog()
        {
            try
            {
                if (MenopauseLogs != null && MenopauseLogs.Id > 0)
                {
                    var todayLog = MenopauseLogs.Entries.FirstOrDefault(x => x.LogDate.Date == DateTime.Today.Date);

                    return todayLog;
                }
            }
            catch
            {
            }
            return null;
        }

        public MenopauseLogEntry GetMenopauseLogByDate(DateTime date)
        {
            try
            {
                if (OvulationLogs != null && OvulationLogs.Id > 0)
                {
                    var log = MenopauseLogs.Entries.FirstOrDefault(x => x.LogDate.Date == date.Date);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting menopause log for date {date}: {ex.Message}");
            }
            return null;
        }

        public MenopauseLogEntry GetMenopauseLogByEntryId(int entryId)
        {
            try
            {
                if (MenopauseLogs != null && MenopauseLogs.Id > 0)
                {
                    var log = MenopauseLogs.Entries.FirstOrDefault(x => x.EntryId == entryId);
                    return log;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting menopause log for Id {entryId}: {ex.Message}");
            }
            return null;
        }

        public async Task<GenericResult> UpdateTodayMenopauseLog(MenopauseLogEntry updateTodayLog)
        {
            try
            {
                updateTodayLog.MenopauseTrackerLogId = (MenopauseLogs != null) ? MenopauseLogs.Id : 0;
                var updateRes = await _moduleLogsApi.UpdateMenopauseLogEntry(UserID, updateTodayLog);

                if (updateRes.Success)
                    await LoadMenopauseLogs(UserID);
                
                return updateRes;
            }
            catch
            {
                return new();
            }
        }
    }
}
