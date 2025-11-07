using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.Notifications
{
    public class NotificationSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string NotificationKey { get; set; } // e.g. "PeriodStart", "MoodLog"
        public string DisplayTitle { get; set; }
        public string MessageTemplate { get; set; }

        public bool IsSubscribed { get; set; }
        public TimeSpan PreferredTime { get; set; } // e.g. 08:30 AM
    }
}
