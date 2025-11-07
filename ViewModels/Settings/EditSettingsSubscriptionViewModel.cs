using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.User;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditSettingsSubscriptionViewModel : BaseViewModel
    {
        public EditFieldProperty SubscriptionStatus { get; set; }
        public EditFieldProperty SubscriptionButton { get; set; }
        public string JoinDate { get; set; }
        public string LastPaymentDate { get; set; }
        public string NextPaymentDate { get; set; }
        public string CancellationDate { get; set; }
        private bool _showCancellationDate;
        public bool ShowCancellationDate
        {
            get => _showCancellationDate;
            set
            {
                SetProperty(ref _showCancellationDate, value);
            }
        }

        private bool _saveButtonEnabled;
        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set
            {
                _saveButtonEnabled = value;
                OnPropertyChanged();
            }
        }


        public EditSettingsSubscriptionViewModel()
        {
            UpdateSubscriptionInformation(LocalStorageService.UserSubscription, false);
        }

        public void UpdateSubscriptionInformation(UserSubscription subscription, bool isUpdate = false)
        {
            try
            {
                var activeStatus = subscription.Status == StatusType.Active;
                if (!isUpdate)
                {
                    JoinDate = $"{subscription.ActiveDate:dd/MM/yyyy}";
                    LastPaymentDate = $"{subscription.NextPaymentDate.AddMonths(-1):dd/MM/yyyy}";
                    NextPaymentDate = $"{subscription.NextPaymentDate:dd/MM/yyyy}";

                    SubscriptionStatus = new EditFieldProperty(subscription.Status.GetDisplayName().ToUpper());
                    SubscriptionStatus.Color = activeStatus ? Colors.DarkGreen : Colors.Maroon;

                    CancellationDate = $"{subscription.LastUpdateDate:dd/MM/yyyy}";
                    ShowCancellationDate = !activeStatus;

                    SubscriptionButton = new EditFieldProperty(activeStatus ? "Cancel Subscription" : "Re-Activate Subscription");
                    SubscriptionButton.Color = activeStatus ? Colors.Crimson : Colors.Green;
                    SubscriptionButton.Color2 = activeStatus ? Colors.Tomato : Colors.SpringGreen;
                }
                else
                {
                    SubscriptionStatus.Value = subscription.Status.GetDisplayName().ToUpper();
                    SubscriptionStatus.Color = activeStatus ? Colors.DarkGreen : Colors.Maroon;

                    CancellationDate = $"{subscription.LastUpdateDate:dd/MM/yyyy}";
                    ShowCancellationDate = !activeStatus;

                    SubscriptionButton.Value = activeStatus ? "Cancel Subscription" : "Re-Activate Subscription";
                    SubscriptionButton.Color = activeStatus ? Colors.Crimson : Colors.Green;
                    SubscriptionButton.Color2 = activeStatus ? Colors.Tomato : Colors.SpringGreen;
                }
                SaveButtonEnabled = isUpdate;
            }
            catch
            {

            }
        }
    }
}
