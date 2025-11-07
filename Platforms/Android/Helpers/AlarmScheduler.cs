using Android.App;
using Android.Content;
using Android.OS;
using Java.Util;
using OvulaeApp.Platforms.Android.Receivers;
using Application = Android.App.Application;

namespace OvulaeApp.Platforms.Android.Helpers
{
    public static class AlarmScheduler
    {
        public static void ScheduleDailyAlarm(TimeSpan preferredTime)
        {
            try
            {
                var context = Application.Context;

                var alarmIntent = new Intent(context, typeof(DailyCycleAlarmReceiver));
                alarmIntent.SetAction("com.ovulae.ovulae.DAILY_CYCLE_ALARM");

                var pendingIntent = PendingIntent.GetBroadcast(
                    context,
                    0,
                    alarmIntent,
                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                );

                var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

                var calendar = Calendar.Instance;
                calendar.Set(CalendarField.HourOfDay, preferredTime.Hours);
                calendar.Set(CalendarField.Minute, preferredTime.Minutes);
                calendar.Set(CalendarField.Second, 0);

                // If it's already past preferred time today, schedule for tomorrow
                if (calendar.TimeInMillis < Java.Lang.JavaSystem.CurrentTimeMillis())
                    calendar.Add(CalendarField.DayOfYear, 1);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.SetExactAndAllowWhileIdle(
                        AlarmType.RtcWakeup,
                        calendar.TimeInMillis,
                        pendingIntent
                    );
                }
                else
                {
                    alarmManager.SetRepeating(
                        AlarmType.RtcWakeup,
                        calendar.TimeInMillis,
                        AlarmManager.IntervalDay,
                        pendingIntent
                    );
                }
            }
            catch (RemoteException ex)
            {
                /*
                // Log to AppCenter or your crash analytics
                Microsoft.AppCenter.Crashes.Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    { "Type", "AlarmManager RemoteException" },
                    { "Context", "ScheduleDailyAlarm" }
                });
                */
                System.Diagnostics.Debug.WriteLine($"AlarmManager RemoteException: {ex}");
            }
            catch (Exception ex)
            {
                /*
                // Catch any other unexpected exceptions
                Microsoft.AppCenter.Crashes.Crashes.TrackError(ex, new Dictionary<string, string>
                {
                    { "Type", "General Exception" },
                    { "Context", "ScheduleDailyAlarm" }
                });
                */
                System.Diagnostics.Debug.WriteLine($"Unexpected error in ScheduleDailyAlarm: {ex}");
            }
        }

    }
}
