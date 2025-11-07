
namespace OvulaeApp.Models.Notifications
{
    public class MenopauseNotificationPreferences
    {
        public bool UseOwnTime { get; set; }
        public TimeSpan PreferredNotificationTime { get; set; }

        public bool NotifyHotFlashes { get; set; }
        public bool NotifyNightSweats { get; set; }
        public bool NotifyMoodSwings { get; set; }

        // Add more settings as needed...
    }
}
