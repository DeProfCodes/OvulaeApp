using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.Notifications;
using OvulaeShared.Models.Notifications;

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

        public PregnancyNotificationsViewModel()
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
                
            }
        }
    }
}
