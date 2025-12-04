
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Helpers.ModuleHelpers;
using OvulaeShared.Models.Diet;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class PregnancyTrackerOnboardModal : ContentView
    {
        private string CurrentSlide = "";

        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public PregnancyTrackerOnboardModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowOnboardingModal()
        {
            CurrentSlide = "DatesOptions";
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

        private void PregnancyQuestionOption_SelectionChanged(object sender, EventArgs e)
        {
            var atLeastOneSelected = PregnancyQuestionOption.SelectedIndex >= 0;
            
            NextBtnTxt.IsEnabled = atLeastOneSelected;
            NextBtnTxt.Opacity = atLeastOneSelected ? 1 : 0.3;
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                KeyboardHelper.Dismiss();

                if (CurrentSlide == "DateOptionSelected")
                {
                    DatesSelectionQuestions.IsVisible = true;
                    LMPOption.IsVisible = false;
                    WeeksDueOption.IsVisible = false;
                    DeliveryDateOption.IsVisible = false;

                    CurrentSlide = "DatesOptions";
                    BackBtnTxt.IsVisible = false;

                    NextBtnTxt.IsEnabled = true;
                    NextBtnTxt.Opacity = 1;
                }
                else if (CurrentSlide == "LastQuestionNavToPregTracker")
                {
                    NavigateToPregnancyTracker.IsVisible = false;

                    var answer = PregnancyQuestionOption.SelectedIndex;

                    LMPOption.IsVisible = answer == 0;
                    WeeksDueOption.IsVisible = answer == 1;
                    DeliveryDateOption.IsVisible = answer == 2;

                    CurrentSlide = "DateOptionSelected";

                    NextBtnTxt.IsEnabled = true;
                    NextBtnTxt.Opacity = 1;
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

                if (CurrentSlide == "DatesOptions")
                {
                    DatesSelectionQuestions.IsVisible = false;
                    LMPOption.IsVisible = false;
                    WeeksDueOption.IsVisible = false;
                    DeliveryDateOption.IsVisible = false;

                    var answer = PregnancyQuestionOption.SelectedIndex;

                    LMPOption.IsVisible = answer == 0;
                    WeeksDueOption.IsVisible = answer == 1;
                    DeliveryDateOption.IsVisible = answer == 2;

                    BackBtnTxt.IsVisible = true;

                    CurrentSlide = "DateOptionSelected";
                }
                else if (CurrentSlide == "DateOptionSelected")
                {
                    LMPOption.IsVisible = false;
                    WeeksDueOption.IsVisible = false;
                    DeliveryDateOption.IsVisible = false;

                    NavigateToPregnancyTracker.IsVisible = true;

                    CurrentSlide = "LastQuestionNavToPregTracker";

                    NextBtnTxt.IsEnabled = true;
                    NextBtnTxt.Opacity = 1;
                }
                else if (CurrentSlide == "LastQuestionNavToPregTracker")
                {
                    var dateAnswer = PregnancyQuestionOption.SelectedIndex;
                    
                    DateTime calculatedLMP = DateTime.Now;

                    if (dateAnswer == 0)
                    {
                        calculatedLMP = LastPeriodDate.SelectedDate;
                    }
                    else if (dateAnswer == 1)
                    {
                        int currentWeek = WeeksDue.Value;
                        calculatedLMP = DateTime.Today.AddDays(-(currentWeek - 1) * 7);
                    }
                    else if (dateAnswer == 2)
                    {
                        calculatedLMP = DeliveryDate.SelectedDate.AddDays(-280);
                    }

                    LocalStorageService.UserCycleProfile.LastPeriodDate = calculatedLMP;
                    var cycleLength = LocalStorageService.UserCycleProfile.CycleLengthDays.Value;
                    
                    LocalStorageService.PregnancyData.PregnancyDataCurrentWeek = (dateAnswer != 1) ? SharedCommonFunctions.GetCurrentCycleDayFromLMP(calculatedLMP, cycleLength) : WeeksDue.Value;

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