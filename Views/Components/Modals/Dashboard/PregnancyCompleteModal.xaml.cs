
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Helpers.Styles;
using OvulaeApp.Helpers.UI;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Models.Diet;

namespace OvulaeApp.Views.Components.Modals.Dashboard
{
    public partial class PregnancyCompleteModal : ContentView
    {
        private string CurrentSlide = "";
        
        private TaskCompletionSource<ModalCloseType> _resultCompletionSource;

        public PregnancyCompleteModal()
        {
            InitializeComponent();
        }

        private void HideAllAndShowThis(VerticalStackLayout vslayout)
        {
            PregnancyEndQuestion.IsVisible = false;
            BirthOption.IsVisible = false;
            BabyGender.IsVisible = false;
            BabyNameQuestion.IsVisible = false;
            MiscarriageDateOption.IsVisible = false;
            OutroOption.IsVisible = false;

            vslayout.IsVisible = true;
        }

        public Task<ModalCloseType> ShowOnboardingModal()
        {
            CurrentSlide = "CompletionTypeOptions";

            MainContainer.HeightRequest = 350;

            MainTitle.Text = "Pregnancy Completed ☑️";
            MainSubTitle.Text = "Let us know how your pregnancy concluded so we can update your tracking and support you best in your next journey.";

            HideAllAndShowThis(PregnancyEndQuestion);

            PregnancyConclusionOption.SelectedIndex = -1;

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

        private void PregnancyConclusionOption_SelectionChanged(object sender, EventArgs e)
        {
            NextBtnTxt.Text = "Next";
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

                var completeType = PregnancyConclusionOption.SelectedIndex;
                NextBtnTxt.Text = "Next";

                if (CurrentSlide == "BirthDateSlide" || CurrentSlide == "MiscarriageDateSlide" || (completeType == 2 && CurrentSlide == "OutroSlide"))
                {
                    HideAllAndShowThis(PregnancyEndQuestion);

                    MainTitle.Text = "Pregnancy Completed ☑️";
                    MainSubTitle.Text = "Let us know how your pregnancy concluded so we can update your tracking and support you best in your next journey.";

                    CurrentSlide = "CompletionTypeOptions";
                    BackBtnTxt.IsVisible = false;

                }
                else if (CurrentSlide == "BabyGenderSlide" || (completeType == 1 && CurrentSlide == "OutroSlide"))
                {
                    if (CurrentSlide == "BabyGenderSlide")
                    {
                        HideAllAndShowThis(BirthOption);

                        CurrentSlide = "BirthDateSlide";
                    }
                    else if (CurrentSlide == "OutroSlide")
                    {
                        HideAllAndShowThis(MiscarriageDateOption);

                        CurrentSlide = "MiscarriageDateSlide";
                    }
                }
                else if (CurrentSlide == "BabyNameSlide")
                {
                    HideAllAndShowThis(BabyGender);

                    CurrentSlide = "BabyGenderSlide";
                }
                else if (CurrentSlide == "OutroSlide")
                {
                    MainContainer.HeightRequest = 350;
                    OutroOption.IsVisible = false;      
                    BabyNameQuestion.IsVisible = true;

                    CurrentSlide = "BabyNameSlide";
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

                if (CurrentSlide == "CompletionTypeOptions")
                {
                    PregnancyEndQuestion.IsVisible = false;

                    var completeType = PregnancyConclusionOption.SelectedIndex;

                    if (completeType == 0)
                    {
                        HideAllAndShowThis(BirthOption);
                        MainTitle.Text = "Congratulations 🥳";
                        MainSubTitle.Text = "Congratulations on your giving birth to your child. Now we are going to ask you optional information, feel free to skip.";

                        CurrentSlide = "BirthDateSlide";
                    }
                    else if (completeType == 1)
                    {
                        HideAllAndShowThis(MiscarriageDateOption);
                        MainTitle.Text = "We're Here for You 💜";
                        MainSubTitle.Text = "We are truly sorry for your loss. If you feel ready, you can share some details to help us support your recovery. This is completely optional, and you can skip anytime.";

                        CurrentSlide = "MiscarriageDateSlide";
                    }
                    else 
                    {
                        HideAllAndShowThis(OutroOption);
                        CurrentSlide = "OutroSlide";
                        NextBtnTxt.Text = "Finish";
                        MainContainer.HeightRequest = 230;
                    }
                    BackBtnTxt.IsVisible = true;
                }
                else if (CurrentSlide == "BirthDateSlide" || CurrentSlide == "MiscarriageDateSlide")
                {
                    HideAllAndShowThis(CurrentSlide == "BirthDateSlide" ? BabyGender : OutroOption);

                    CurrentSlide = (CurrentSlide == "BirthDateSlide") ? "BabyGenderSlide" : "OutroSlide";
                    NextBtnTxt.Text = CurrentSlide == "OutroSlide" ? "Finish" : "Next";
                }
                else if (CurrentSlide == "BabyGenderSlide")
                {
                    HideAllAndShowThis(BabyNameQuestion);

                    CurrentSlide = "BabyNameSlide";
                }
                else if (CurrentSlide == "BabyNameSlide")
                {
                    HideAllAndShowThis(OutroOption);

                    CurrentSlide = "OutroSlide";
                    NextBtnTxt.Text = "Finish";
                    MainContainer.HeightRequest = 230;
                }
                else if (CurrentSlide == "OutroSlide")
                {
                    HideAllAndShowThis(PregnancyEndQuestion);
                    
                    var answer = PregnancyConclusionOption.SelectedIndex;

                    if (answer == 0)
                    {
                        LocalStorageService.PregnancyData.BabyBirthDate = BabyBirthDate.SelectedDate;
                        LocalStorageService.PregnancyData.BabyGender = BabyGenderOption.SelectedValue;
                        LocalStorageService.PregnancyData.BabyName = BabysNameEntry.Text;
                    }
                    else if (answer == 1)
                    {
                        LocalStorageService.PregnancyData.MiscarriageDate = MiscarriageDate.SelectedDate;
                    }
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