using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class EditCycleLengthsModal : ContentView
    {
        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public EditCycleLengthsModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowEditCycleDaysModal()
        {
            this.IsVisible = true;
            _resultCompletionSource = new TaskCompletionSource<ModalCloseType>();

            LastPeriodDate.SelectedDate = LocalStorageService.UserCycleProfile.LastPeriodDate ?? DateTime.Today;
            CycleLengthValue.Value = LocalStorageService.UserCycleProfile.CycleLengthDays.Value;
            CycleDurationValue.Value = LocalStorageService.UserCycleProfile.PeriodLengthDays.Value;

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
            KeyboardHelper.Dismiss();
            await HideAsync(ModalCloseType.CloseButton);
        }

        private async void NextButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                LocalStorageService.UserCycleProfile.LastPeriodDate = LastPeriodDate.SelectedDate;
                LocalStorageService.UserCycleProfile.CycleLengthDays = CycleLengthValue.Value;
                LocalStorageService.UserCycleProfile.PeriodLengthDays = CycleDurationValue.Value;

                await HideAsync(ModalCloseType.Accept);
            }
            catch
            {
                await HideAsync(ModalCloseType.Error);
            }
        }
    }
}