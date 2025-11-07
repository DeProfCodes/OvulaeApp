using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using OvulaeApp.Models.Notifications;
using Plugin.LocalNotification;

namespace OvulaeApp.Services.Notifications
{
    public interface INotificationSchedulerService
    {
        Task ScheduleDailyPeriodOvulationNotificationsAsync(PeriodOvulationNotificationPreferences preferences);
        
        Task SendNowAsync(NotificationRequest request);

        public Task SchedulePlanAsync(List<ScheduledNotification> plan);

        public void ClearScheduledNotifications();

    }
}
