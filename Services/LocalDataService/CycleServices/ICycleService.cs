using OvulaeApp.Helpers.Enums;
using OvulaeApp.Models.Notifications;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.User;

namespace OvulaeApp.Services.LocalDataService.CycleServices
{
    public interface ICycleService
    {
        public Task<bool> LoadCycleDataAsync();

        public bool LoadCycleData(UserCycleProfile userCycleProfile);

        public int GetCycleLength();

        public int GetPeriodLength();

        public DateTime GetLastMenstrualPeriodDate();

        public ModuleType GetModuleType();

        public int GetCurrentPregnancyWeekFromDueDate(DateTime dueDate);

        public int GetCurrentPregnancyWeekFromLMP();

        public int GetCurrentCycleDay();

        public int GetStartPhaseDay(OvulationPhase phase);

        public int GetPhaseLength(OvulationPhase phase);

        public int GetPhaseDayWithinPhase(OvulationPhase phase, DateTime referenceDate);

        public OvulationPhase GetPhaseForCycleDay(int currentCycleDay);

        public OvulationPhase GetNextPhase(OvulationPhase currentPhase);

        public bool IsFirstDayOfPhase(DateTime date);

        public int GetCycleDayForDate(DateTime date);

        public (DateTime StartDate, DateTime EndDate) CalculatePhaseDateRange(OvulationPhase phase, DateTime referenceDate, int cycleOffset = 0, bool alignToToday = false);

        public OvulationPhase GetCurrentPhase();

        public OvulationPhase GetPhaseForDate(DateTime date);

        public (DateTime Start, DateTime End) GetNextFertileWindow();

        public List<ScheduledNotification> BuildDayPlan(DateTime date, PeriodOvulationNotificationPreferences prefs);

    }
}
