using OvulaeApp.Helpers.Controls;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class PeriodTrackerOnboardModal : ContentView
    {
        private string CurrentSlide = "";

        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public PeriodTrackerOnboardModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowOnboardingModal(string title= "Period Tracker Setup")
        {
            ModalTitle.Text = title;

            CurrentSlide = "LMPDate";
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
            KeyboardHelper.Dismiss();
            await HideAsync(ModalCloseType.CloseButton);
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                if (CurrentSlide == "CycleLength")
                {
                    LMPOption.IsVisible = true;
                    CycleLength.IsVisible = false;
                    CycleDuration.IsVisible = false;

                    CurrentSlide = "LMPDate";
                    BackBtnTxt.IsVisible = false;
                }
                else if (CurrentSlide == "CycleDuration")
                {
                    LMPOption.IsVisible = false;
                    CycleLength.IsVisible = true;
                    CycleDuration.IsVisible = false;

                    CurrentSlide = "CycleLength";
                    NextBtnTxt.Text = "Next";
                }
            }
            catch 
            {
                await HideAsync(ModalCloseType.Error);
            }
        }

        private async void NextButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                if (CurrentSlide == "LMPDate")
                {
                    LMPOption.IsVisible = false;
                    CycleLength.IsVisible = true;
                    CycleDuration.IsVisible = false;

                    CurrentSlide = "CycleLength";
                    BackBtnTxt.IsVisible = true;
                }
                else if (CurrentSlide == "CycleLength")
                {
                    LMPOption.IsVisible = false;
                    CycleLength.IsVisible = false;
                    CycleDuration.IsVisible = true;

                    CurrentSlide = "CycleDuration";
                    NextBtnTxt.Text = "Finish";
                }
                else if (CurrentSlide == "CycleDuration")
                {
                    LocalStorageService.UserCycleProfile.LastPeriodDate = LastPeriodDate.SelectedDate;
                    LocalStorageService.UserCycleProfile.CycleLengthDays = CycleLengthValue.Value;
                    LocalStorageService.UserCycleProfile.PeriodLengthDays = CycleDurationValue.Value;

                    LocalStorageService.PeriodTrackerSet = true;

                    await HideAsync(ModalCloseType.Accept);
                }
            }
            catch
            {
                await HideAsync(ModalCloseType.Error);
            }
        }
    }
}