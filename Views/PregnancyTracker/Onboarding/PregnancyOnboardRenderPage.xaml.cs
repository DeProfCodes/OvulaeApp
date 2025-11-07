using OvulaeApp.Helpers.Styles;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.ViewModels.GoalSetting;
using OvulaeApp.ViewModels.PregnancyTracker;
using OvulaeApp.Views.Components.Controls;
using OvulaeApp.Views.Components.Shared;
using OvulaeApp.Views.OvulationTracker.Onboarding;

namespace OvulaeApp.Views.PregnancyTracker.Onboarding
{
    public partial class PregnancyOnboardRenderPage : ContentPage
    {
        private readonly PregnancyOnboardViewModel vm;

        public PregnancyOnboardRenderPage()
        {
            InitializeComponent();

            vm = new PregnancyOnboardViewModel(AppSpinner);

            BindingContext = vm;

            MyYearPicker.SetBinding(
                YearPicker.SelectedYearProperty,
                new Binding("SelectedYear", source: vm, mode: BindingMode.TwoWay)
            );

            ShowIntraMarketingPage();
        }

        private void ShowIntraMarketingPage()
        {
            BenefitsAdPage.IsVisible = vm.CurrentIndex == 0;
            DashboardAdPage.IsVisible = vm.CurrentIndex == 3;
            CalendarAdPage.IsVisible = vm.CurrentIndex == 6;
            ChatBotAdPage.IsVisible = vm.CurrentIndex == 11;
            EducationBooksAdPage.IsVisible = vm.CurrentIndex == 14;
            PartnerShareAdPage.IsVisible = vm.CurrentIndex == 16;
        }

        protected override bool OnBackButtonPressed()
        {
            try
            {
                if (vm.CurrentIndex > 0)
                {
                    vm.GoBack();
                    ShowIntraMarketingPage();
                    return true;
                }
                if (vm.CurrentIndex == 0)
                {
                    Shell.Current.GoToAsync(nameof(PregnancyOnboardWelcomePage));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
            return base.OnBackButtonPressed();
        }

        private void GoBackDotButtonTapped(object sender, TappedEventArgs e)
        {
            try
            {
                vm.GoBack();
                ShowIntraMarketingPage();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
        }

        private async void YearBorder_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                await MyYearPicker.ShowAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Year picker failed to open, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while opening year picker.", "OK");
            }
        }

        private void MeasurementType_Tapped(object sender, TappedEventArgs e)
        {
            try
            {
                if (sender is Border entry)
                {
                    if (entry == msKG)
                    {
                        msKG.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msKG.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msKGLabel.TextColor = Colors.White;
                        msLBS.Background = Brush.White;
                        msLBS.Stroke = Brush.White;
                        msLBSLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        vm.WeightUnit = "kg";
                    }
                    else if (entry == msLBS)
                    {
                        msKG.Background = Brush.White;
                        msKG.Stroke = Brush.White;
                        msKGLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msLBS.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBS.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msLBSLabel.TextColor = Colors.White;
                        vm.WeightUnit = "lbs";
                    }
                    else if (entry == msCM)
                    {
                        msCM.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCM.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msCMLabel.TextColor = Colors.White;
                        msIN.Background = Brush.White;
                        msIN.Stroke = Brush.White;
                        msINLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        vm.HeightUnit = "cm";
                    }
                    else if (entry == msIN)
                    {
                        msCM.Background = Brush.White;
                        msCM.Stroke = Brush.White;
                        msCMLabel.TextColor = OvulaeColors.COLOR.ThemeGray2;
                        msIN.Background = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msIN.Stroke = OvulaeColors.BRUSH.BrushThemeClrMain;
                        msINLabel.TextColor = Colors.White;
                        vm.HeightUnit = "ft/in";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to adjust unit measure, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while adjusting unit measure.", "OK");
            }
        }

        private void OnBoardingQuestionsChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender is SelectComponent)
                {
                    var select = (SelectComponent)sender;
                    var atLeastOneOptionSelected = select.SelectedIndex >= 0;

                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected;
                }
                else if (sender is SelectComponentCheck)
                {
                    var select = (SelectComponentCheck)sender;
                    var atLeastOneOptionSelected = select.SelectedItems.Count > 0;

                    vm.CurrentSlide.IsNextEnabled = atLeastOneOptionSelected;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to answer navigating questions, error: {ex.Message}");
            }
        }

        private async void NextOnboardBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (vm.CurrentIndex == 1)
                {
                    HandlePregnancyQuestion();
                }
                else if (vm.CurrentIndex == 8)
                {
                    HandlePregnancyDateOptions();
                }
                else if (vm.CurrentIndex == 9)
                {
                    Preferences.Set("DueDate", PregnancyDueDate.SelectedDate);
                    
                    vm.Slides[vm.CurrentIndex].ShowSlide = false;
                    vm.CurrentIndex = 11;
                    vm.Slides[vm.CurrentIndex].ShowSlide = true;
                    ShowIntraMarketingPage();
                }
                else if (vm.CurrentIndex == 10)
                {
                    Preferences.Set("LastPeriodDate", LastPeriodDate.SelectedDate);
                    vm.GoNext();
                    ShowIntraMarketingPage();
                }
                else if (vm.CurrentIndex == vm.Slides.Count - 1)
                {
                    /*
                    LocalStorageService.OnboardingData.QuestionAndAnswers.AddRange(
                        new QnA { Question = "PregnantQuestion", Answer = "Yes, I'm Pregnant" },
                        new QnA { Question = "FirstPregnancyQuestion", Answer = FirstPregnancyOptions.SelectedValue },
                        new QnA { Question = "HelpWithPregnancyQuestion", Answer = string.Join(",", AssistanceOptionsCheck.SelectedItems) }
                    );
                    */
                    LocalStorageService.UserCycleProfile.FirstPregnancy = FirstPregnancyOptions.SelectedValue == "Yes, this is my first";

                    LocalStorageService.UserBodyMetrics.Year = vm.SelectedYear;
                    LocalStorageService.UserBodyMetrics.Weight = WeightValue.Value;
                    LocalStorageService.UserBodyMetrics.WeightUnit = vm.WeightUnit;
                    LocalStorageService.UserBodyMetrics.Height = HeightValue.Value;
                    LocalStorageService.UserBodyMetrics.HeightUnit = vm.HeightUnit;

                    if (BloodTypeOption.SelectedIndex >= 0)
                    {
                        var bloodType = BloodTypeOption.SelectedValue;
                        LocalStorageService.UserBodyMetrics.BloodGroup = bloodType.Substring(0, bloodType.Length - 1);
                        LocalStorageService.UserBodyMetrics.RhFactor = $"{bloodType[bloodType.Length - 1]}";
                    }

                    await AppSpinner.ShowSpinnerAsync();

                    await Shell.Current.GoToAsync(nameof(PregnancyOnboardFinishPage));

                    await AppSpinner.HideSpinnerAsync();
                }
                else
                {
                    vm.GoNext();
                    ShowIntraMarketingPage();
                }

                DotBackButton.IsVisible = vm.CurrentIndex > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to next page, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating onboarding questions.", "OK");
            }
        }

        private async void HandlePregnancyQuestion()
        {
            try
            {
                int answer = PregnancyQuestionOption.SelectedIndex;

                if (answer == 0)
                {
                    vm.GoNext();
                    ShowIntraMarketingPage();
                }
                else if (answer == 1)
                {
                    await AppSpinner.ShowSpinnerAsync();
                    await Shell.Current.GoToAsync(nameof(OvulationOnboardWelcomePage));
                    await AppSpinner.HideSpinnerAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to next page, error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while navigating onboarding questions.", "OK");
            }
        }

        private void HandlePregnancyDateOptions()
        {
            int answer = PregnancyDateSelectQuestion.SelectedIndex;

            if (answer == 0)
            {
                vm.GoNext();
                ShowIntraMarketingPage();
            }
            else if (answer == 1)
            {
                vm.Slides[vm.CurrentIndex].ShowSlide = false;
                vm.CurrentIndex = 10;
                vm.Slides[vm.CurrentIndex].ShowSlide = true;
            }
        }

        private void IntraMarketingPageContainer_BackClicked(object sender, EventArgs e)
        {
            try
            {
                if (vm.CurrentIndex == 11)
                {
                    vm.Slides[vm.CurrentIndex].ShowSlide = false;
                    
                    vm.CurrentIndex = PregnancyDateSelectQuestion.SelectedIndex == 1 ? 10 : 9;

                    vm.Slides[vm.CurrentIndex].ShowSlide = true;
                }
                else
                {
                    vm.GoBack(); 
                }
                ShowIntraMarketingPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failure, error: {ex.Message}");
            }
        }
    }
}
