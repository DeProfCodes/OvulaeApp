using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.Notifications
{
    public class DeferredNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string NotificationKey { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsSent { get; set; }
    }
}
