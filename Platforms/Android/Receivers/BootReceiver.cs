using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Platforms.Android.Helpers;

namespace OvulaeApp.Platforms.Android.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {

                if (intent.Action == Intent.ActionBootCompleted)
                {
                    // Load preferred time from storage
                    var prefs = FileReaderHelper.GetAllFileData<NotificationsGroup>("NotificationsPreferences.json").Result;
                    var preferred = prefs?.PeriodOvulationNotification?.PreferredNotificationTime ?? TimeSpan.FromHours(12);
                    AlarmScheduler.ScheduleDailyAlarm(preferred);
                }
            }
            catch 
            {
            }
        }
    }
}
