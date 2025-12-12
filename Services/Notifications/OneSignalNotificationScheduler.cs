//using Com.OneSignal; // Android binding namespace
using Newtonsoft.Json;
//using OneSignalSDK.DotNet;
using OvulaeApp.Models.Notifications;

namespace OvulaeApp.Services.Notifications
{
    public class OneSignalNotificationScheduler
    {
        public static async Task ScheduleDailyNotificationsAsync(List<ScheduledNotification> notifications)
        {
            foreach (var notif in notifications)
            {
                await ScheduleLocalOneSignalNotificationAsync(notif);
            }
        }

        private static async Task ScheduleLocalOneSignalNotificationAsync(ScheduledNotification notif)
        {
            try
            {
                var delaySeconds = (notif.ScheduledTime - DateTime.Now).TotalSeconds;
                if (delaySeconds < 0) delaySeconds = 5; // fallback safety

                var notification = new
                {
                    headings = new { en = notif.Title },
                    contents = new { en = notif.Message },
                    send_after = DateTime.UtcNow.AddSeconds(delaySeconds).ToString("yyyy-MM-dd HH:mm:ss 'GMT'"),
                    ios_badgeType = "Increase",
                    ios_badgeCount = 1,
                    priority = 10,
                    android_channel_id = "default",
                    android_sound = "default",
                    app_id = "YOUR_ONESIGNAL_APP_ID", // optional if initialized globally
                    target_channel = "local_device_only"
                };

                var payload = JsonConvert.SerializeObject(notification);
                /*
                OneSignal.PostNotification(payload,
                    success =>
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ OneSignal scheduled: {notif.Title}");
                    },
                    error =>
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ OneSignal schedule failed: {error}");
                    }
                );
                */

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ OneSignal schedule exception: {ex.Message}");
            }
        }
    }
}
