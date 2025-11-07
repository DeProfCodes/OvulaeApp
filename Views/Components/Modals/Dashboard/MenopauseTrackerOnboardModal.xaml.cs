using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.HealthProfile;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class MenopauseTrackerOnboardModal : ContentView
    {
        private string CurrentSlide = "";

        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public MenopauseTrackerOnboardModal()
        {
            InitializeComponent();
        }

        public Task<ModalCloseType> ShowOnboardingModal()
        {
            CurrentSlide = "SymptomsSlide";
            MainContainer.HeightRequest = 350;

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

        private void SymptomsQuestionOptions_SelectionChanged(object sender, EventArgs e)
        {
            var atLeastOneSelected = SymptomsQuestionOptions.SelectedIndex >= 0;
        }

        private async void BackButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (CurrentSlide == "LMPSection")
                {
                    SymptomsSelectionQuestions.IsVisible = true;
                    LMPOption.IsVisible = false;
                    
                    CurrentSlide = "SymptomsSlide";
                    BackBtnTxt.IsVisible = false;

                    NextButtonLabel.Text = "Next";
                    MainContainer.HeightRequest = 350;
                }
                else if (CurrentSlide == "OutroSection")
                {
                    NavigateToMenopauseTracker.IsVisible = false;
                    
                    if (SymptomsQuestionOptions.SelectedValues.Contains("12+ months since last period"))
                    {
                        CurrentSlide = "SymptomsSlide";
                        SymptomsSelectionQuestions.IsVisible = true;
                        MainContainer.HeightRequest = 350;
                    }
                    else
                    {
                        CurrentSlide = "LMPSection";
                        LMPOption.IsVisible = true;
                        MainContainer.HeightRequest = 300;
                    }
                    NextButtonLabel.Text = "Next";
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
                if (CurrentSlide == "SymptomsSlide")
                {
                    SymptomsSelectionQuestions.IsVisible = false;
                    
                    BackBtnTxt.IsVisible = true;

                    if (SymptomsQuestionOptions.SelectedValues.Contains("12+ months since last period"))
                    {
                        CurrentSlide = "OutroSection";
                        NextButtonLabel.Text = "Finish";
                        NavigateToMenopauseTracker.IsVisible = true;
                        MainContainer.HeightRequest = 230;
                    }
                    else
                    {
                        LMPOption.IsVisible = true;
                        CurrentSlide = "LMPSection";
                        MainContainer.HeightRequest = 300;
                    }
                }
                else if (CurrentSlide == "LMPSection")
                {
                    LMPOption.IsVisible = false;
                    NavigateToMenopauseTracker.IsVisible = true;

                    CurrentSlide = "OutroSection";
                    NextButtonLabel.Text = "Finish";
                    MainContainer.HeightRequest = 230;
                }
                else if (CurrentSlide == "OutroSection")
                {
                    //Irregular Period
                    if (SymptomsQuestionOptions.SelectedValues.Contains("Irregular periods (frequently)"))
                        LocalStorageService.UserCycleProfile.PeriodIrregularityType = PeriodIrregularityType.FrequentlyIrregular;
                    
                    else if (SymptomsQuestionOptions.SelectedValues.Contains("Irregular periods (occasional)"))
                        LocalStorageService.UserCycleProfile.PeriodIrregularityType = PeriodIrregularityType.OccasionallyIrregular;
                    
                    else
                        LocalStorageService.UserCycleProfile.PeriodIrregularityType = PeriodIrregularityType.Regular;

                    //Symptoms
                    if (SymptomsQuestionOptions.SelectedValues.Contains("Hot flashes"))
                        LocalStorageService.UserCycleProfile.Symptoms.Add(SymptomsTypes.HotFlashes.GetDisplayName());

                    if (SymptomsQuestionOptions.SelectedValues.Contains("Night sweats"))
                        LocalStorageService.UserCycleProfile.Symptoms.Add(SymptomsTypes.NightSweats.GetDisplayName());

                    if (SymptomsQuestionOptions.SelectedValues.Contains("Mood swings"))
                        LocalStorageService.UserCycleProfile.Symptoms.Add(SymptomsTypes.MoodChanges.GetDisplayName());

                    //Treatments
                    if (SymptomsQuestionOptions.SelectedValues.Contains("Using hormone therapy"))
                        LocalStorageService.UserCycleProfile.Treatments.Add(TreatmentTypes.HRT.GetDisplayDescription());

                    //LMP
                    if (SymptomsQuestionOptions.SelectedValues.Contains("12+ months since last period"))
                        LocalStorageService.UserCycleProfile.LastPeriodDate = DateTime.Now.AddMonths(-12);
                    
                    else
                        LocalStorageService.UserCycleProfile.LastPeriodDate = LastPeriodDate.SelectedDate;
                    
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