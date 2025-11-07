using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.Notifications;

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

        private readonly INotificationsPreferenceService _notificationsPrefServ;

        public MenopauseNotificationsViewModel(INotificationsPreferenceService notificationsPrefServ)
        {
            _notificationsPrefServ = notificationsPrefServ;
            LoadNotificationsData();
        }

        private async void LoadNotificationsData()
        {
            try
            {
                Notifications = await _notificationsPrefServ.GetMenopauseNotifications();
                ShowManualTime = Notifications?.UseOwnTime ?? false;
            }
            catch
            {
                // Add logging if needed
            }
        }
    }
}
