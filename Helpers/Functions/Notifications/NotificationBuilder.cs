using OvulaeApp.Models.Notifications;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.Helpers.Functions.Notifications
{
    public static class NotificationBuilder
    {
        // Static template definitions
        private static readonly List<NotificationTemplate> Templates = new()
        {
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyPeriodStart), Title = "⏰ Period Reminder", Message = "Your period may start soon. Track your symptoms today." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyFertileWindow), Title = "🌱 Fertile Window", Message = "You're in your fertile window. Track your ovulation signs." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyOvulationDay), Title = "🥚 Ovulation Alert", Message = "Today is likely your ovulation day. Stay aware of your cycle." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyPmsSymptoms), Title = "⚡ PMS Reminder", Message = "Feeling off? Track your PMS symptoms for better insights." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyLogMood), Title = "😊 Mood Check-In", Message = "How are you feeling today? Log your mood in 2 taps." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyHydration), Title = "💧 Hydration Boost", Message = "Drink some water. Your body and mind will thank you!" },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyCycleTips), Title = "📘 Cycle Tip", Message = "Learn something new about your cycle today." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyDailyAffirmation), Title = "💬 Daily Affirmation", Message = "You are strong, capable, and supported. 🌸" },

            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyFollicularPhaseTips), Title = "🌿 Follicular Phase", Message = "You're in your follicular phase. Time to energize and move!" },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyOvulationDayTips), Title = "✨ Ovulation Tips", Message = "Today’s tip: Increase hydration and embrace your glow!" },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyLutealPhaseTips), Title = "🌙 Luteal Phase", Message = "Luteal phase has started. Prioritize rest and magnesium-rich foods." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyCervicalMucusChanges), Title = "🔍 Cervical Mucus", Message = "Notice changes in discharge? It could indicate fertility." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyTemperatureShiftReminder), Title = "🌡️ BBT Reminder", Message = "Remember to check your basal temperature this morning." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifySexTimingAdvice), Title = "❤️ Conception Timing", Message = "Best time for conception? Stay in sync with your ovulation signs." },

            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyFertilityNutritionTips), Title = "🥦 Fertility Food Tip", Message = "Today's focus: Leafy greens for hormone balance." },
            new NotificationTemplate { Key = nameof(PeriodOvulationNotificationPreferences.NotifyMindfulnessAndRelaxation), Title = "🧘 Mindful Moment", Message = "Take 5 deep breaths and pause. Your peace matters." },
        };

        public static List<ScheduledNotification> BuildFromPreferences(PeriodOvulationNotificationPreferences prefs, DateTime date, int maxPerDay = 2)
        {
            var enabledKeys = prefs.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && (bool)(p.GetValue(prefs) ?? false))
                                   .Select(p => p.Name).ToHashSet();

            var matchedTemplates = Templates.Where(t => enabledKeys.Contains(t.Key)).Take(maxPerDay).ToList();

            var scheduledList = new List<ScheduledNotification>();

            for (int i = 0; i < matchedTemplates.Count; i++)
            {
                var template = matchedTemplates[i];
                scheduledList.Add(new ScheduledNotification
                {
                    NotificationKey = template.Key,
                    Title = template.Title,
                    Message = template.Message,
                    ScheduledTime = date.Date + prefs.PreferredNotificationTime + TimeSpan.FromMinutes(i * 5)
                });
            }

            return scheduledList;
        }

        public static int GenerateNotificationId(DateTime time)
        {
            // Generate a repeat-safe ID (e.g. based on timestamp hash)
            return Math.Abs(time.ToString("yyyyMMddHHmm").GetHashCode());
        }
    }
}
