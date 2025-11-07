using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;

namespace OvulaeApp.ViewModels.Subscription
{
    public class PaywallViewModel : BaseViewModel
    {
        private string _monthlyPrice;
        public string MonthlyPrice 
        {
            get => _monthlyPrice;
            set
            {
                _monthlyPrice = value;
                OnPropertyChanged();
            }
        }

        private string _dailyPrice;
        public string DailyPrice 
        { 
            get => _dailyPrice;
            set
            {
                _dailyPrice = value;
                OnPropertyChanged();
            }
        }

        private bool _showFreeTrial;
        public bool ShowFreeTrial
        {
            get => _showFreeTrial;
            set
            {
                _showFreeTrial = value;
                OnPropertyChanged();
            }
        }

        private bool _showPremium;
        public bool ShowPremium
        {
            get => _showPremium;
            set
            {
                _showPremium = value;
                OnPropertyChanged();
            }
        }

        private MobileDeviceType deviceType;

        public PaywallViewModel(MobileDeviceType deviceType)
        {
            this.deviceType = deviceType;
            UpdateLocalPrices();
        }

        private void UpdateLocalPrices()
        {
            if (deviceType == MobileDeviceType.Android)
            {
                UpdateLocalPricesAndroid();
            }

            ShowFreeTrial = LocalStorageService.UserSubscription.Status == StatusType.Pending;
            ShowPremium = !ShowFreeTrial;
        }

        private void UpdateLocalPricesAndroid()
        {
            var isZA = LocalStorageService.UserDetails.CountryCode == "+27";
            MonthlyPrice = isZA ? "R95.00" : "$5.99";
            DailyPrice = $"≈ {(isZA ? "R3.16" : "$0.20")} per day";
        }
    }
}
