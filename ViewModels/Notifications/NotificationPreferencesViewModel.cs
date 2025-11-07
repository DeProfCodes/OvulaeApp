using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Notifications
{
    public class NotificationPreferencesViewModel : BaseViewModel
    {
        public TimeSpan PreferredTime { get; set; } = TimeSpan.FromHours(8);

        public Dictionary<string, bool> NotificationToggles { get; set; } = new()
        {
            { "PeriodStart", true },
            { "OvulationDay", false },
            { "MoodLog", true },
        };
    }
}
