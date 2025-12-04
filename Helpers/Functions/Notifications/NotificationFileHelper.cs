using OvulaeShared.Enums.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Functions.Notifications
{
    public static class NotificationFileHelper
    {
        private const string PENDING_NOTIFICATION_FILE = "PendingNotification.json";

        public class PendingNotificationData
        {
            public string Module { get; set; }
            public string EntryId { get; set; }
        }

        public static async Task SavePendingNotificationAsync(string module, string entryId)
        {
            var data = new PendingNotificationData
            {
                Module = module,
                EntryId = entryId
            };
            await FileReaderHelper.SaveDataAsync(data, PENDING_NOTIFICATION_FILE);
        }

        public static async Task<PendingNotificationData> GetAndClearPendingNotificationAsync()
        {
            var data = await FileReaderHelper.GetAllFileData<PendingNotificationData>(PENDING_NOTIFICATION_FILE);
            if (data != null)
            {
                // Clear the file after reading
                await FileReaderHelper.SaveDataAsync(new PendingNotificationData(), PENDING_NOTIFICATION_FILE);
            }
            return data;
        }
    }
}
