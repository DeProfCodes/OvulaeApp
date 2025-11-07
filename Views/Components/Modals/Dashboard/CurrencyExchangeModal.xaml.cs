
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Models.Diet;

namespace OvulaeApp.Views.Components.Modals
{
    public partial class CurrencyExchangeModal : ContentView
    {
        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public CurrencyExchangeModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowExchangeRateModal(double amountUSD, double amountZAR)
        {
            var isZA = LocalStorageService.UserDetails.CountryCode == "+27";

            IntroText.Text = $"You're about to pay USD {amountUSD:N2}, which converts to approximately {amountZAR:N2} ZAR";
            ZARAmount.Text = $"{amountZAR:N2}";
            DollarAmount.Text = $"{amountUSD:N2}";

            this.IsVisible = true;
            _resultCompletionSource = new TaskCompletionSource<ModalCloseType>();

            this.FadeTo(1, 200);

            return _resultCompletionSource.Task;
        }

        public async Task HideAsync(ModalCloseType result = ModalCloseType.None)
        {
            await this.FadeTo(0, 200);
            this.IsVisible = false;

            _resultCompletionSource?.TrySetResult(result);
        }

        private async void CloseButton_Tapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.CloseButton);
        }

        private async void NoButtonTapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.Reject);
        }

        private async void YesButtonTapped(object sender, TappedEventArgs e)
        {
            await HideAsync(ModalCloseType.Accept);
        }
    }
}