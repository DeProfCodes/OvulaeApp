using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Services.Notifications
{
    public interface IFcmNotificationService
    {
        public Task InitializeFirebase();
    }
}
