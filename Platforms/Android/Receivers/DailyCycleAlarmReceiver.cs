using Android.App;
using Android.Content;
using Android.Runtime;
using OvulaeApp.Platforms.Android.Helpers;
using OvulaeApp.Services.Jobs;
using OvulaeApp.Services.Jobs.Daily;
using OvulaeApp.Services.Notifications;
using Shiny.Jobs;

namespace OvulaeApp.Platforms.Android.Receivers
{
    [Register("com.ovulae.ovulae.DailyCycleAlarmReceiver")]
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "com.ovulae.ovulae.DAILY_CYCLE_ALARM" })]
    public class DailyCycleAlarmReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            try
            {
                var action = intent.Action;

                if (action == Intent.ActionBootCompleted || action == Intent.ActionMyPackageReplaced)
                {
                    // Reschedule alarm on boot
                    var prefs = await AppServiceHelper.Services.GetService<INotificationsPreferenceService>()?.GetPeriodOvulationNotifications();
                    if (prefs != null)
                    {
                        AlarmScheduler.ScheduleDailyAlarm(prefs.PreferredNotificationTime);
                    }
                }

                // Trigger job
                var job = AppServiceHelper.Services.GetService<DailyCycleJob>();
                if (job != null)
                {
                    var info = new JobInfo("manual", typeof(DailyCycleJob));
                    await job.Run(info, CancellationToken.None);
                }
            }
            catch
            {
                
            }
        }
    }
}
