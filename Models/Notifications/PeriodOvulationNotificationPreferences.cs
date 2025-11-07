
namespace OvulaeApp.Models.Notifications
{
    public class PeriodOvulationNotificationPreferences
    {
        // 🔔 General
        public bool UseOwnTime { get; set; } = false;
        public TimeSpan PreferredNotificationTime { get; set; } = TimeSpan.FromHours(12);

        // 🩸 Period Tracking Notifications
        public bool NotifyPeriodStart { get; set; } = true;
        public bool NotifyFertileWindow { get; set; } = true;
        public bool NotifyOvulationDay { get; set; } = true;
        public bool NotifyPmsSymptoms { get; set; } = false;
        public bool NotifyLogMood { get; set; } = false;
        public bool NotifyHydration { get; set; } = false;
        public bool NotifyCycleTips { get; set; } = true;
        public bool NotifyDailyAffirmation { get; set; } = false;

        // 🌸 Ovulation Tracking Notifications
        public bool NotifyFollicularPhaseTips { get; set; } = false;
        public bool NotifyOvulationDayTips { get; set; } = false;
        public bool NotifyLutealPhaseTips { get; set; } = false;
        public bool NotifyCervicalMucusChanges { get; set; } = false;
        public bool NotifyTemperatureShiftReminder { get; set; } = false;
        public bool NotifySexTimingAdvice { get; set; } = false;

        // 🧠 Lifestyle & Education
        public bool NotifyFertilityNutritionTips { get; set; } = false;
        public bool NotifyMindfulnessAndRelaxation { get; set; } = false;
    }
}
