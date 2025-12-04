using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.Notifications;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.ViewModels.MenopauseTracker
{
    public class MenopauseNotificationsViewModel : BaseViewModel
    {
        private MenopauseNotificationPreferences _notifications;
        public MenopauseNotificationPreferences Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged();
            }
        }

        private bool _showManualTime;
        public bool ShowManualTime
        {
            get => _showManualTime;
            set
            {
                _showManualTime = value;
                OnPropertyChanged();
            }
        }

        public MenopauseNotificationsViewModel()
        {
            LoadNotificationsData();
        }

        private async void LoadNotificationsData()
        {
            try
            {
                Notifications = new();
                ShowManualTime = Notifications?.UseOwnTime ?? false;
            }
            catch
            {
                // Add logging if needed
            }
        }
    }
}
