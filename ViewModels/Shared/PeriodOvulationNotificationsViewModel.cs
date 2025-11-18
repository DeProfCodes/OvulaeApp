using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.Notifications;
using OvulaeShared.Models.Notifications;

namespace OvulaeApp.ViewModels.Shared
{
    public class PeriodOvulationNotificationsViewModel : BaseViewModel
    {
        private PeriodOvulationNotificationPreferences _notifications;
        public PeriodOvulationNotificationPreferences Notifications
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

        public PeriodOvulationNotificationsViewModel(INotificationsPreferenceService notificationsPrefServ)
        {
            _notificationsPrefServ = notificationsPrefServ;

            LoadNotificationsData();
        }

        private async void LoadNotificationsData()
        {
            try
            {
                Notifications = await _notificationsPrefServ.GetPeriodOvulationNotifications();
                ShowManualTime = Notifications?.UseOwnTime ?? false;
            }
            catch
            {
                
            }
        }
    }
}
