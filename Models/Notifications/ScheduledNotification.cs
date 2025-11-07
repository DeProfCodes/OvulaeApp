using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Functions.Notifications;
using Plugin.LocalNotification;

namespace OvulaeApp.Models.Notifications
{
    public class ScheduledNotification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string NotificationKey { get; set; }

        public NotificationRequest ToRequest()
        {
            return new NotificationRequest
            {
                NotificationId = NotificationBuilder.GenerateNotificationId(ScheduledTime),
                Title = Title,
                Description = Message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = ScheduledTime,
                    RepeatType = NotificationRepeat.No
                }
            };
        }
    }
}
