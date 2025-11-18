using OvulaeShared.Enums.App;
using OvulaeShared.Enums;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.LocalDataService.PeriodTrackerServices;
using OvulaeApp.Services.LocalDataService.OvulationServices;
using OvulaeShared.Models.User;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.Services.LocalDataService.CycleServices
{
    public class CycleService : ICycleService
    {
        private readonly IUserLocalService _usersServe;

        private int CycleLength;
        private int PeriodLength;
        private DateTime LmpDate;
        private ModuleType moduleType;

        private const int OvulationLength = 3;
        private const int LutealLength = 14;

        public CycleService(IUserLocalService usersServe)
        {
            _usersServe = usersServe;
        }

        public async Task<bool> LoadCycleDataAsync()
        {
            try
            {
                var cycleProfile = await _usersServe.GetUserCycleDetails();

                CycleLength = cycleProfile.CycleLengthDays != null ? cycleProfile.CycleLengthDays.Value : 28;
                PeriodLength = cycleProfile.PeriodLengthDays != null ? cycleProfile.PeriodLengthDays.Value : 5;
                LmpDate = cycleProfile.LastPeriodDate != null ? cycleProfile.LastPeriodDate.Value : DateTime.Now;
                moduleType = EnumHelper.GetEnumValueFromName<ModuleType>(cycleProfile.OvulaePrimaryGoal);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadCycleData(UserCycleProfile cycleProfile)
        {
            try
            {
                CycleLength = cycleProfile.CycleLengthDays != null ? cycleProfile.CycleLengthDays.Value : 28;
                PeriodLength = cycleProfile.PeriodLengthDays != null ? cycleProfile.PeriodLengthDays.Value : 5;
                LmpDate = cycleProfile.LastPeriodDate != null ? cycleProfile.LastPeriodDate.Value : DateTime.Now;
                moduleType = EnumHelper.GetEnumValueFromName<ModuleType>(cycleProfile.OvulaePrimaryGoal);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetCycleLength()
        {
            return CycleLength;
        }

        public int GetPeriodLength()
        {
            return PeriodLength;
        }

        public DateTime GetLastMenstrualPeriodDate()
        {
            return LmpDate;
        }

        public ModuleType GetModuleType()
        {
            return moduleType;
        }

        public int GetCurrentPregnancyWeekFromDueDate(DateTime dueDate)
        {
            DateTime estimatedLMP = dueDate.AddDays(-280);
            int totalDays = (DateTime.Today - estimatedLMP).Days;
            int week = (int)Math.Ceiling(totalDays / 7.0);
            return Math.Clamp(week, 1, 42);
        }

        public int GetCurrentPregnancyWeekFromLMP()
        {
            int totalDays = (DateTime.Today - LmpDate).Days;
            int week = (int)Math.Ceiling(totalDays / 7.0);
            return Math.Clamp(week, 1, 42);
        }

        public int GetCurrentCycleDay()
        {
            int daysSinceLmp = (DateTime.Today - LmpDate).Days;
            return (daysSinceLmp % CycleLength) + 1;
        }

        public int GetStartPhaseDay(OvulationPhase phase)
        {
            return phase switch
            {
                OvulationPhase.Menstrual => 1,
                OvulationPhase.Follicular => PeriodLength + 1,
                OvulationPhase.Ovulation => CycleLength - (LutealLength + OvulationLength) + 1,
                OvulationPhase.Luteal => CycleLength - LutealLength + 1,
                _ => 1
            };
        }

        public int GetPhaseLength(OvulationPhase phase)
        {
            return phase switch
            {
                OvulationPhase.Menstrual => PeriodLength,
                OvulationPhase.Follicular => CycleLength - (PeriodLength + OvulationLength + LutealLength),
                OvulationPhase.Ovulation => OvulationLength,
                OvulationPhase.Luteal => LutealLength,
                _ => 0
            };
        }

        public int GetPhaseDayWithinPhase(OvulationPhase phase, DateTime referenceDate)
        {
            int startPhaseDay = GetStartPhaseDay(phase);
            int daysSinceLmp = (referenceDate.Date - LmpDate.Date).Days;
            int cycleDay = ((daysSinceLmp % CycleLength) + CycleLength) % CycleLength + 1; // Corrected
            int dayInPhase = cycleDay - startPhaseDay + 1;
            return Math.Clamp(dayInPhase, 1, GetPhaseLength(phase));
        }

        public OvulationPhase GetPhaseForCycleDay(int currentCycleDay)
        {
            int menstrualStart = GetStartPhaseDay(OvulationPhase.Menstrual);
            int follicularStart = GetStartPhaseDay(OvulationPhase.Follicular);
            int ovulationStart = GetStartPhaseDay(OvulationPhase.Ovulation);
            int lutealStart = GetStartPhaseDay(OvulationPhase.Luteal);

            if (currentCycleDay >= menstrualStart && currentCycleDay < follicularStart) return OvulationPhase.Menstrual;
            if (currentCycleDay >= follicularStart && currentCycleDay < ovulationStart) return OvulationPhase.Follicular;
            if (currentCycleDay >= ovulationStart && currentCycleDay < lutealStart) return OvulationPhase.Ovulation;
            else return OvulationPhase.Luteal;
        }

        public OvulationPhase GetNextPhase(OvulationPhase currentPhase)
        {
            return currentPhase switch
            {
                OvulationPhase.Menstrual => OvulationPhase.Follicular,
                OvulationPhase.Follicular => OvulationPhase.Ovulation,
                OvulationPhase.Ovulation => OvulationPhase.Luteal,
                OvulationPhase.Luteal => OvulationPhase.Menstrual, // loop back to start
                _ => OvulationPhase.Menstrual
            };
        }

        public bool IsFirstDayOfPhase(DateTime date)
        {
            var cycleDay = GetCycleDayForDate(date);
            var phase = GetPhaseForCycleDay(cycleDay);
            var previousPhase = GetPhaseForCycleDay(cycleDay - 1);
            return phase != previousPhase;
        }

        public int GetCycleDayForDate(DateTime date)
        {
            int daysSinceLmp = (date.Date - LmpDate.Date).Days;

            int cycleDay = ((daysSinceLmp % CycleLength) + CycleLength) % CycleLength + 1;

            return cycleDay;
        }

        public (DateTime StartDate, DateTime EndDate) CalculatePhaseDateRange(OvulationPhase phase, DateTime referenceDate, int cycleOffset = 0, bool alignToToday = false)
        {
            int startPhaseDay = GetStartPhaseDay(phase);
            int phaseLength = GetPhaseLength(phase);
            int daysSinceLmp = (referenceDate.Date - LmpDate.Date).Days;
            DateTime phaseStartDate;

            if (alignToToday)
            {
                int daysIntoCycle = daysSinceLmp % CycleLength;
                DateTime currentCycleStart = referenceDate.AddDays(-daysIntoCycle);
                phaseStartDate = currentCycleStart.AddDays(startPhaseDay - 1);
            }
            else
            {
                DateTime cycleBaseDate = LmpDate.AddDays(cycleOffset * CycleLength);
                phaseStartDate = cycleBaseDate.AddDays(startPhaseDay - 1);
            }

            DateTime phaseEndDate = phaseStartDate.AddDays(phaseLength - 1);
            return (phaseStartDate, phaseEndDate);
        }

        public OvulationPhase GetCurrentPhase()
        {
            int currentCycleDay = GetCurrentCycleDay();
            int ovulationStart = GetStartPhaseDay(OvulationPhase.Ovulation);
            int lutealStart = GetStartPhaseDay(OvulationPhase.Luteal);

            if (currentCycleDay >= 1 && currentCycleDay < GetStartPhaseDay(OvulationPhase.Follicular))
            {
                return OvulationPhase.Menstrual;
            }
            else if (currentCycleDay >= GetStartPhaseDay(OvulationPhase.Follicular) && currentCycleDay < ovulationStart)
            {
                return OvulationPhase.Follicular;
            }
            else if (currentCycleDay >= ovulationStart && currentCycleDay < lutealStart)
            {
                return OvulationPhase.Ovulation;
            }
            else
            {
                return OvulationPhase.Luteal;
            }
        }

        public static (DateTime PregnancyStartDate, DateTime EstimatedConceptionDate) CalculatePregnancyStartDates(DateTime lmpDate)
        {
            DateTime pregnancyStartDate = lmpDate.Date;
            DateTime estimatedConceptionDate = lmpDate.AddDays(14).Date;
            return (pregnancyStartDate, estimatedConceptionDate);
        }

        public OvulationPhase GetPhaseForDate(DateTime date)
        {
            int cycleDay = GetCycleDayForDate(date);
            return GetPhaseForCycleDay(cycleDay);
        }

        public (DateTime Start, DateTime End) GetNextFertileWindow()
        {
            DateTime today = DateTime.Today;

            // Find which cycle we're in (0 = current, 1 = next cycle, etc.)
            int daysSinceLmp = (today - LmpDate).Days;
            int currentCycleNumber = daysSinceLmp / CycleLength;

            for (int offset = 0; offset <= 1; offset++)
            {
                int cycle = currentCycleNumber + offset;
                var ovulationStart = LmpDate.AddDays(cycle * CycleLength + GetStartPhaseDay(OvulationPhase.Ovulation) - 1);
                var fertileStart = ovulationStart.AddDays(-5);
                var fertileEnd = ovulationStart.AddDays(1);

                if (fertileEnd >= today)
                    return (fertileStart, fertileEnd);
            }

            // fallback (rare case)
            return (today, today);
        }

        public List<ScheduledNotification> BuildDayPlan(DateTime date, PeriodOvulationNotificationPreferences prefs)
        {
            var dataSource = (moduleType == ModuleType.PeriodTracker) ? PeriodTrackerService.PeriodDataStore : OvulationService.OvulationDataStore;

            var result = new List<ScheduledNotification>();
            var phase = GetPhaseForDate(date);
            var phaseData = dataSource.FirstOrDefault(x => x.ModulePhase == phase);

            if (phaseData == null)
                return result;

            var tipTexts = phaseData.PhaseDetails?.Tips ?? new List<string>();

            bool isFirstDay = IsFirstDayOfPhase(date);
            var notifyTime = prefs.UseOwnTime ? prefs.PreferredNotificationTime : TimeSpan.FromHours(12);

            if (isFirstDay)
            {
                result.Add(new ScheduledNotification
                {
                    Title = $"{phaseData.PhaseDetails.PhaseTitle} Begins",
                    Message = phaseData.PhaseSummary,
                    ScheduledTime = date.Date + notifyTime
                });
            }
            else
            {
                var dayInPhase = GetPhaseDayWithinPhase(phase, date);
                int tipsPerDay = 2;

                for (int i = 0; i < tipsPerDay; i++)
                {
                    int tipIndex = ((dayInPhase - 2) * tipsPerDay + i) % tipTexts.Count;

                    if (tipTexts.Count > 0)
                    {
                        result.Add(new ScheduledNotification
                        {
                            Title = $"{phaseData.PhaseDetails.PhaseTitle} Tip 🌸",
                            Message = tipTexts[tipIndex],
                            ScheduledTime = date.Date + notifyTime + TimeSpan.FromHours(4 * i)
                        });
                    }
                }
            }

            return result;
        }
    }
}
