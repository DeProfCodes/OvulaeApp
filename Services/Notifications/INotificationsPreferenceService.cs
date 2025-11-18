using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Models.Notifications;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.Services.Notifications
{
    public interface INotificationsPreferenceService
    {
        public Task<PeriodOvulationNotificationPreferences> GetPeriodOvulationNotifications();

        public Task<PregnancyNotificationPreferences> GetPregnancyNotifications();

        public Task<bool> UpdatePeriodOvulationNotifications(PeriodOvulationNotificationPreferences newPreferences);

        public Task<bool> UpdatePregnancyNotifications(PregnancyNotificationPreferences newPreferences);

        public Task<MenopauseNotificationPreferences> GetMenopauseNotifications();

        public Task<bool> UpdateMenopauseNotifications(MenopauseNotificationPreferences newPreferences);

        public Task<List<ScheduledNotification>> BuildFullWeekPlanForPregnancyNotifications();
    }
}
