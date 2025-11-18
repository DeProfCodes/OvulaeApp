using OvulaeApp.Helpers.Functions.Notifications;
using OvulaeApp.Models.Notifications;
using OvulaeShared.Models.Notifications;
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
            try
            {
                var todayNotifications = NotificationBuilder.BuildFromPreferences(preferences, DateTime.Today, maxPerDay: 2);

                foreach (var notification in todayNotifications)
                {
                    await ScheduleAsync(notification.Title, notification.Message, notification.ScheduledTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Scheduling daily notifications: {ex.Message}");
            }
        }

        public async Task SendNowAsync(NotificationRequest request)
        {
            try
            {
                await _notificationService.Show(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Sending notification now: {ex.Message}");
            }
        }

        private async Task ScheduleAsync(string title, string body, DateTime scheduledTime)
        {
            if (scheduledTime < DateTime.Now)
                scheduledTime = scheduledTime.AddDays(1);

            try
            {
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

                await _notificationService.Show(notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Scheduling notification: {ex.Message}");
            }
        }

        public async Task SchedulePlanAsync(List<ScheduledNotification> plan)
        {
            if (plan == null || plan.Count == 0)
                return;

            try
            {
                foreach (var notif in plan)
                {

                    var request = new NotificationRequest
                    {
                        NotificationId = NotificationBuilder.GenerateNotificationId(notif.ScheduledTime),
                        Title = notif.Title,
                        Description = notif.Message,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notif.ScheduledTime,
                            NotifyRepeatInterval = TimeSpan.Zero,
                            RepeatType = NotificationRepeat.No
                        }
                    };

                    await _notificationService.Show(request);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Scheduling local notification plan: {ex.Message}");
            }
        }

        public async Task SendNowOrScheduleAsync(ScheduledNotification notif)
        {
            var delaySeconds = (notif.ScheduledTime - DateTime.Now).TotalSeconds;
            if (delaySeconds <= 0)
            {
                // Send immediately
                await SendNowAsync(new NotificationRequest
                {
                    Title = notif.Title,
                    Description = notif.Message,
                    NotificationId = NotificationBuilder.GenerateNotificationId(notif.ScheduledTime)
                });
            }
            else
            {
                // Schedule for later
                await SchedulePlanAsync(new List<ScheduledNotification> { notif });
            }
        }

        public void ClearScheduledNotifications()
        {

            try
            {
                _notificationService.CancelAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Clearing notifications: {ex.Message}");
            }
        }
    }
}
