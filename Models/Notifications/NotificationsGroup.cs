using OvulaeShared.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.Notifications
{
    public class NotificationsGroup
    {
        public PeriodOvulationNotificationPreferences PeriodOvulationNotification { get; set; }

        public PregnancyNotificationPreferences PregnancyNotifications { get; set; }

        public MenopauseNotificationPreferences MenopauseNotification { get; set; }
    }
}
