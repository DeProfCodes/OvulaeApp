using OvulaeShared.Models.PeriodTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Functions.Modules
{
    public class PcosEndoHelperFunctions
    {
        public enum PcosOrEndoWeight
        {
            PCOS,
            ENDO
        }

        public static bool HasPcosOrEndoWeightRecordedThisMonth(List<PeriodLogEntry> PeriodLogs, PcosOrEndoWeight pcosOrEndo)
        {
            try
            {
                if (PeriodLogs == null)
                    return false;

                // Get the year and month of the target date
                var targetDate = DateTime.Now;
                int targetYear = targetDate.Year;
                int targetMonth = targetDate.Month;

                bool hasWeightRecorded = PeriodLogs.Any(log =>
                {
                    if (log.LogDate.Year == targetYear && log.LogDate.Month == targetMonth)
                    {
                        var weight = pcosOrEndo == PcosOrEndoWeight.PCOS ? log.PcosWeight : log.EndoWeight;
                        return weight > 0.0;
                    }
                    return false;
                });

                return hasWeightRecorded;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking PCOS/ENDO weight for month {DateTime.Now:yyyy-MM}: {ex.Message}");
                return false;
            }
        }

        public static bool HasPcosWeightRecordedThisMonth(List<PeriodLogEntry> PeriodLogs)
        {
            return HasPcosOrEndoWeightRecordedThisMonth(PeriodLogs, PcosOrEndoWeight.PCOS);
        }

        public static bool HasEndoWeightRecordedThisMonth(List<PeriodLogEntry> PeriodLogs)
        {
            return HasPcosOrEndoWeightRecordedThisMonth(PeriodLogs, PcosOrEndoWeight.ENDO);
        }
    }
}
