using OvulaeApp.Helpers.Functions.Notifications;
using OvulaeApp.Models.Notifications;
using Plugin.LocalNotification;

namespace OvulaeApp.Services.Notifications
{
    public class NotificationSchedulerService : INotificationSchedulerService
    {
        private readonly INotificationService _notificationService;

        public NotificationSchedulerService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task ScheduleDailyPeriodOvulationNotificationsAsync(PeriodOvulationNotificationPreferences preferences)
        {
            var todayNotifications = NotificationBuilder.BuildFromPreferences(preferences, DateTime.Today, maxPerDay: 2);

            foreach (var notification in todayNotifications)
            {
                Schedule(notification.Title, notification.Message, notification.ScheduledTime);
            }
        }

        public async Task SendNowAsync(NotificationRequest request)
        {
#if ANDROID || IOS
            await _notificationService.Show(request);
#endif
        }

        private void Schedule(string title, string body, DateTime scheduledTime)
        {
            if (scheduledTime < DateTime.Now)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

#if ANDROID || IOS
            
            var notification = new NotificationRequest
            {
                NotificationId = NotificationBuilder.GenerateNotificationId(scheduledTime),
                Title = title,
                Description = body,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = scheduledTime,
                    NotifyRepeatInterval = TimeSpan.Zero,
                    RepeatType = NotificationRepeat.No
                }
            };

            _notificationService.Show(notification);
#endif
        }

        public async Task SchedulePlanAsync(List<ScheduledNotification> plan)
        {
            foreach (var notification in plan)
                await _notificationService.Show(notification.ToRequest());
        }

        public void ClearScheduledNotifications()
        {
#if ANDROID || IOS
            _notificationService.CancelAll();
#endif
        }
    }
}
