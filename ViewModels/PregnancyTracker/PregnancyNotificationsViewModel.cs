using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.Notifications;

namespace OvulaeApp.ViewModels.PregnancyTracker
{
    public class PregnancyNotificationsViewModel : BaseViewModel
    {
        private PregnancyNotificationPreferences _notifications;
        public PregnancyNotificationPreferences Notifications
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

        public PregnancyNotificationsViewModel(INotificationsPreferenceService notificationsPrefServ)
        {
            _notificationsPrefServ = notificationsPrefServ;

            LoadNotificationsData();
        }

        private async void LoadNotificationsData()
        {
            try
            {
                Notifications = await _notificationsPrefServ.GetPregnancyNotifications();
                ShowManualTime = Notifications?.UseOwnTime ?? false;
            }
            catch
            {
                
            }
        }
    }
}
